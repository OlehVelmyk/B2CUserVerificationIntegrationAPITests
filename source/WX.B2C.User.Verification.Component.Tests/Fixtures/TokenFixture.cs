using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest;

namespace WX.B2C.User.Verification.Component.Tests.Fixtures
{
    public static class TokenFixture
    {
        private static readonly Random WeakRandom = new Random(42);

        public static string GenerateToken(Guid userId, string ipAddress) => CreateJwt(userId, ipAddress);

        public static TokenCredentials GenerateCredentials(Guid userId, string ipAddress)
        {
            var jwt = CreateJwt(userId, ipAddress);
            return new TokenCredentials(jwt, "Bearer");
        }

        private static string CreateJwt(Guid ownerId, string clientIp)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var signingCredentials = GenerateSigningCredentials();
            var token = tokenHandler.CreateJwtSecurityToken("wx_api",
                                                            "wx_b2c_user_verification",
                                                            new ClaimsIdentity(),
                                                            DateTime.UtcNow,
                                                            DateTime.UtcNow.AddDays(1),
                                                            signingCredentials: signingCredentials);
            token.Payload["OwnerId"] = ownerId;
            token.Payload["ClientIp"] = clientIp;
            return tokenHandler.WriteToken(token);
        }

        private static SigningCredentials GenerateSigningCredentials()
        {
            var oneTimeKeyData = new byte[32];
            WeakRandom.NextBytes(oneTimeKeyData);
            return new SigningCredentials(new SymmetricSecurityKey(oneTimeKeyData),
                                          SecurityAlgorithms.HmacSha256Signature);
        }
    }
}