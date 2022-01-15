using UmesiServer.Data;
using UmesiServer.Hubs.NotificationHub;
using UmesiServer.Middleware;
using UmesiServer.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("UmesiPolicy", opt =>
    {
        opt.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
    });
});
builder.Services.AddSignalR();

builder.Services.AddSingleton<UnitOfWork>();

Settings.EncryptKey = builder.Configuration["EncryptionSecretKey"];

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("UmesiPolicy");

app.UseAuthorization();

app.UseMiddleware<AuthMiddleware>();

app.MapControllers();

app.MapHub<NotificationHub>("/hub/notifications");

app.Run();
