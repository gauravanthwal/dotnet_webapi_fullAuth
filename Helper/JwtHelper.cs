using FullAuth.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FullAuth.Helper
{
    public class JwtHelper: IJwtHelper
    {
        private readonly IConfiguration _config;
        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        // GENERATE ACCESS TOKEN
        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ??
                throw new Exception("Jwt key is missing!")));

            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sid, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Email), // email will be unique thats why name = email 
                new Claim(ClaimTypes.Role, user.Role), // Specify user role here
                new Claim("Date", DateTime.Now.ToString()),
            };


            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims.ToArray(),
                notBefore:DateTime.Now,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credential
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // GENERATE REFRESH TOKEN
        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }


        // GET PRINCIPAL FROM EXPIRED TOKEN
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ??
                    throw new Exception("Jwt key is missing!"))),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken securityToken;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                throw new SecurityTokenException("Invalid Token");
            }
            return principal;
        }

    }
}
