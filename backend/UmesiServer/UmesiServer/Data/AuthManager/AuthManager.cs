using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UmesiServer.DTOs.Records;
using UmesiServer.Exceptions;
using UmesiServer.Models;

namespace UmesiServer.Data.AuthManager
{
    public class AuthManager : IAuthManager
    {
        private ConnectionMultiplexer _redis;
        private Func<string, string> userKeyBuilder;
        private Func<User, string> encryptStringBuilder;
        private Aes _aesAlg;

        public AuthManager(ConnectionMultiplexer redis)
        {
            _redis = redis;
            userKeyBuilder = (string username) => $"{username}:LoginKey";
            encryptStringBuilder = (User user) => $"{user.Username}|{user.Name}|{user.Surname}";
            _aesAlg = Aes.Create();
            _aesAlg.Key = Convert.FromBase64String(Settings.Settings.EncryptKey);
            _aesAlg.GenerateIV();
        }

        public async Task<string> Login(LoginDto creds)
        {
            string token = null;
            try
            {
                IDatabase db = _redis.GetDatabase();
                string redisUser = (await db.StringGetAsync(creds.Username)).ToString();
                if (string.IsNullOrEmpty(redisUser))
                    throw new HttpResponseException(404, "User not found");
                User user = JsonSerializer.Deserialize<User>(redisUser);
                if (user.Password != creds.Password)
                    throw new HttpResponseException(401, "Passwords do not match");
                token = Convert.ToBase64String(MakeRedisValue(encryptStringBuilder(user)));
                await db.StringSetAsync(userKeyBuilder(user.Username), token);
                await db.KeyExpireAsync(userKeyBuilder(user.Username), TimeSpan.FromHours(2));

                return token;
            }
            catch (Exception ex)
            {
                if (ex is HttpResponseException)
                    throw new HttpResponseException((ex as HttpResponseException).Status, ex.Message);
                Console.Error.WriteLine(ex.Message);
            }
            return token;
        }

        public async Task<bool> IsLogedIn(string token)
        {
            try
            {
                if (token == null)
                    throw new HttpResponseException(400, "Token not set");
                string tokenValue = GetStringValueFromRedisValue(Convert.FromBase64String(token));
                List<string> tokenParts = tokenValue.Split("|").ToList();
                if (tokenParts.Count == 0)
                    throw new HttpResponseException(400, "Token is not the right format");
                IDatabase db = _redis.GetDatabase();
                if (!(await db.KeyExistsAsync(userKeyBuilder(tokenParts[0]))))
                    return false;
                string redisValue = (await db.StringGetAsync(userKeyBuilder(tokenParts[0]))).ToString();
                string storedValue = GetStringValueFromRedisValue(Convert.FromBase64String(redisValue));
                    
                return tokenValue == storedValue;
            }
            catch (Exception ex)
            {
                if (ex is HttpResponseException)
                    throw new HttpResponseException((ex as HttpResponseException).Status, ex.Message);
                Console.Error.WriteLine(ex.Message);
            }
            return false;
        }

        private byte[] MakeRedisValue(string key)
        {
            byte[] encrypted;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = _aesAlg.CreateEncryptor(_aesAlg.Key, _aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(key);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private string GetStringValueFromRedisValue(byte[] encriptedKey)
        {
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = _aesAlg.CreateDecryptor(_aesAlg.Key, _aesAlg.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(encriptedKey))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }
    }
}
