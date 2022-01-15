using StackExchange.Redis;
using UmesiServer.Models;

namespace UmesiServer.DTOs.Records
{
    public record LoginDto(string Username, string Password);
    public record TokenDto(string Token);
    public record RecipeWithIndex(int Index, Recipe Recipe);
    public record UpdateCommentDto(int RecipeId, int Index, Comment Comment);
    public record NotificationDto(string Title, string Message);
    public record ConnIdAAndSubscriberRecord(string ConnectionId, ISubscriber subscriber);
}
