using System.Security.Claims;

namespace DotnetApi.Extension;

public static class ClaimPrincipalExtension
{
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        return user.Claims.FirstOrDefault(claim => claim.Type is ClaimTypes.Email or ClaimTypes.Name)?.Value ??
               string.Empty;
    }

    public static bool TryGetUserEmail(this ClaimsPrincipal user, out string email)
    {
        email = user.GetUserEmail();

        return !string.IsNullOrEmpty(email);
    }

    public static string[] GetUserRoles(this ClaimsPrincipal user)
    {
        return user.Claims.Where(claim => claim.Type is ClaimTypes.Role).Select(x => x.Value).ToArray();
    }

    public static bool TryGetUserRoles(this ClaimsPrincipal user, out string[] roles)
    {
        roles = user.GetUserRoles();

        return roles.Length > 0;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        const string roleName = "admin";

        return user.TryGetUserRoles(out var roles) && roles.Any(x => x == roleName);
    }

    public static bool IsAdminOrVerify(this ClaimsPrincipal user, Func<bool> verifyAction)
    {
        return user.IsAdmin() || verifyAction.Invoke();
    }

    public static bool IsAdminOrVerify(this ClaimsPrincipal user, bool verify)
    {
        return user.IsAdmin() || verify;
    }
}