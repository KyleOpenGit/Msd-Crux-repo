using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using MSD.Crux.API.Models;

namespace MSD.Crux.API.Helpers
{
    /// <summary>
    /// JWT 생성 도우미 클래스
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// 특정 유저의 클레임정보를 가진 토큰을 생성한다.
        /// </summary>
        /// <param name="user">User 테이블 엔티티 객체</param>
        /// <param name="configuration">IConfiguration 객체</param>
        /// <returns>유저토큰(JwtSecurityToken을 직렬화한 문자열)</returns>
        public static string GenerateToken(User user, IConfiguration configuration)
        {
            string privateKeyPem = configuration["Jwt:PrivateKey"];
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            RsaSecurityKey signingKey = new RsaSecurityKey(rsa);
            SigningCredentials credentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

            // 클레임 생성
            Claim[] claims = new[]
                             {
                                 new Claim(ClaimTypes.NameIdentifier, user.LoginId!), new Claim(ClaimTypes.Name, user.Name),
                                 new Claim("Employee Number", user.EmployeeNumber.ToString()), new Claim(ClaimTypes.Role, user.Roles)
                             };

            // JWT 생성
            var token = new JwtSecurityToken(issuer: configuration["Jwt:Issuer"],
                                             audience: configuration["Jwt:Audience"],
                                             claims: claims,
                                             expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:TokenLifetimeMinutes"])),
                                             signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// appsettings.json에 기록된 .pem 타입 퍼블릭키 문자열로 RSA공개키 객체를 만든다
        /// </summary>
        /// <param name="configuration">Iconfiguration 객체 </param>
        /// <returns>RSA공개키객체</returns>
        public static RsaSecurityKey GetPublicKey(IConfiguration configuration)
        {
            // RSA Public Key 로드
            string publicKeyPem = configuration["Jwt:PublicKey"];
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.ToCharArray());
            return new RsaSecurityKey(rsa);
        }
    }
}
