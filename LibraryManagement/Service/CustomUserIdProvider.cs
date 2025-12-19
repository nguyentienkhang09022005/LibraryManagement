using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LibraryManagement.Service
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var userId = connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                userId = connection.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            }

            if (string.IsNullOrEmpty(userId))
            {
                userId = connection.User?.FindFirst("sub")?.Value;
            }

            return userId;
        }
    }
}