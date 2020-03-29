using System;
using Microsoft.AspNetCore.SignalR;

namespace online_avalon_web
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public CustomUserIdProvider()
        {
        }

        public string GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();
            var username = httpContext.Request.Query["username"];
            var gameId = httpContext.Request.Query["publicGameId"];
            return GetUserId(username, gameId);
        }

        public static string GetUserId(string username, string publicGameId)
        {
            return $"{publicGameId}:{username}";
        }
    }
}
