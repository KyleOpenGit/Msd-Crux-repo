using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MSD.Crux.Common;

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
        RSA rsa = CreateRsaFromPem(configuration["Jwt:PrivateKey"]);
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
        RSA rsa = CreateRsaFromPem(configuration["Jwt:PublicKey"]);
        return new RsaSecurityKey(rsa);
    }

    /// <summary>
    /// appsettings.json에 기록된 .pem 타입 퍼블릭키 문자열을 그대로 반환한다. DTO 전송용
    /// </summary>
    /// <param name="configuration">구성 파일 객체</param>
    /// <returns>pem 파일 형식의 공개 키 문자열</returns>
    public static string GetPublicKeyAsString(IConfiguration configuration) => configuration["Jwt:PublicKey"];

    /// <summary>
    /// JWT 토큰을 검증하고 클레임 정보를 반환합니다.
    /// </summary>
    /// <param name="token">검증할 JWT 토큰</param>
    /// <param name="configuration">구성 파일에서 JWT 검증 정보를 가져올 객체</param>
    /// <returns>유효한 토큰인 경우 ClaimsPrincipal 객체, 그렇지 않으면 null</returns>
    public static ClaimsPrincipal? ValidateToken(string token, IConfiguration configuration)
    {
        RSA rsa = CreateRsaFromPem(configuration["Jwt:PublicKey"]);

        TokenValidationParameters vParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };

        JwtSecurityTokenHandler handler = new();

        try
        {
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring(7); // "Bearer " 접두사 제거
            }
            return handler.ValidateToken(token, vParameters, out _);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// .pem 형식의 키 문자열로부터 RSA 객체를 생성한다
    /// </summary>
    /// <param name="pemKey">.pem 형식의 키 문자열 (공개 또는 비공개)</param>
    /// <returns>생성된 RSA 객체</returns>
    private static RSA CreateRsaFromPem(string pemKey)
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(pemKey.ToCharArray());
        return rsa;
    }
}
