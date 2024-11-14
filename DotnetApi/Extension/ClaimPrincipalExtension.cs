using System.Security.Claims;

namespace DotnetApi.Extension;

public static class ClaimPrincipalExtension
{
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        return user.Claims.FirstOrDefault(c => c.Type is ClaimTypes.Email or ClaimTypes.Name)?.Value ?? string.Empty;
    }

    public static bool TryGetUserEmail(this ClaimsPrincipal user, out string email)
    {
        email = GetUserEmail(user);

        return !string.IsNullOrEmpty(email);
    }
}