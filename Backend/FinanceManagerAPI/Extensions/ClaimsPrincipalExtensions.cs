using System.Security.Claims;

namespace FinanceManagerAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(claimValue, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid JWT claims.");
        }

        return userId;
    }
}
