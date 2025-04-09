using FullAuth.Entities;
using System.Security.Claims;

namespace FullAuth.Helper
{
    public interface IJwtHelper
    {
        string GenerateJwtToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
