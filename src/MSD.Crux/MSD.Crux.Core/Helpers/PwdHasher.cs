using System.Security.Cryptography;
using System.Text;

namespace MSD.Crux.Core.Helpers;

/// <summary>
/// 비밀번호를 해싱하고 검증하는 헬퍼 클래스
/// </summary>
public class PwdHasher
{
    /// <summary>
    /// 비밀번호를 PBKDF2 알고리즘으로 해싱하고, 랜덤 Salt와 함께 반환.
    /// </summary>
    /// <param name="password">사용자가 입력한 비밀번호 (Plain Text)</param>
    /// <param name="salt">랜덤으로 생성된 16바이트(22글자) Base64 Salt</param>
    /// <returns>해싱된 비밀번호 (Base64 문자열)</returns>
    public static string GenerateHash(string password, out string salt)
    {
        // 16바이트 랜덤 Salt 생성
        salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

        // PBKDF2 해싱 수행
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 100000, HashAlgorithmName.SHA256))
        {
            byte[] hashedPassword = pbkdf2.GetBytes(32); // SHA256 해시: 32바이트
            return Convert.ToBase64String(hashedPassword);
        }
    }

    /// <summary>
    /// 사용자가 입력한 비밀번호가 저장된 해시와 일치하는지 검증.
    /// </summary>
    /// <param name="password">사용자가 입력한 비밀번호 (Plain Text)</param>
    /// <param name="salt">DB에 저장된 Salt (22글자)</param>
    /// <param name="storedHash">DB에 저장된 해싱된 비밀번호 (Base64)</param>
    /// <returns>비밀번호 검증 결과 (일치하면 true, 그렇지 않으면 false)</returns>
    public static bool VerifyHash(string password, string salt, string storedHash)
    {
        // 사용자가 입력한 비밀번호와 Salt를 사용해 해시 생성
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt), 100000, HashAlgorithmName.SHA256))
        {
            byte[] hashedPassword = pbkdf2.GetBytes(32); // SHA256 해시: 32바이트
            string computedHash = Convert.ToBase64String(hashedPassword);

            return computedHash == storedHash;
        }
    }
}
