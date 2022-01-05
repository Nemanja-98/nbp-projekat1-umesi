namespace UmesiServer.DTOs.Records
{
    public record LoginDto(string Username, string Password);
    public record TokenDto(string Token);
}
