using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InvestmentControl.Application.Common.Interfaces;

namespace InvestmentControl.Infrastructure.Services;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("Пользователь не аутентифицирован.");
            return int.Parse(userIdClaim);
        }
    }

    public string Role
    {
        get
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("roles")?.Value;
            if (string.IsNullOrEmpty(roleClaim))
                throw new UnauthorizedAccessException("Роль пользователя не определена.");
            return roleClaim;
        }
    }
}
