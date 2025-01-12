using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MSD.Crux.Core.Helpers
{
    /// <summary>
    /// JWT 생성, 검증 도우미 클래스
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// 클레임 정보를 가진 토큰을 생성한다.
        /// </summary>
        /// <param name="claims">JWT에 포함할 클레임</param>
        /// <param name="configuration">구성 파일에서 JWT 관련 정보를 가져올 IConfiguration 객체</param>
        /// <returns><see cref="JwtSecurityToken"/>을 JWT로 직렬화한 문자열</returns>
        public static string GenerateToken(IEnumerable<Claim> claims, IConfiguration configuration)
        {
            string privateKeyPem = configuration["Jwt:PrivateKey"];
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            RsaSecurityKey signingKey = new RsaSecurityKey(rsa);
            SigningCredentials credentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

            // JWT 생성
            var jwtSecurityToken = new JwtSecurityToken(issuer: configuration["Jwt:Issuer"],
                                                        audience: configuration["Jwt:Audience"],
                                                        expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:TokenLifetimeMinutes"])),
                                                        claims: claims,
                                                        signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
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
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.ToCharArray());
            return new RsaSecurityKey(rsa);
        }

        /// <summary>
        /// appsettings.json에 기록된 .pem 타입 퍼블릭키 문자열을 그대로 반환한다. DTO 전송용
        /// </summary>
        /// <param name="configuration">구성 파일 객체</param>
        /// <returns>pem 파일 형식의 공개 키 문자열</returns>
        public static string GetPublicKeyAsString(IConfiguration configuration) => configuration["Jwt:PublicKey"];
    }
}
