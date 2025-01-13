using MSD.Crux.Core.Models;

namespace MSD.Crux.Core.Helpers;

public static class GenericHelper
{
    /// <summary>
    /// Employee 테이블의 Year, Gender, Sequence를 기반으로 고유한 Employee Number를 생성
    /// </summary>
    /// <param name="year">입사 연도</param>
    /// <param name="gender">성별 코드</param>
    /// <param name="sequence">입사 순번</param>
    /// <returns>조합된 Employee Number: 연도(4자리) + 성별(1자리) + 순번(4자리)</returns>
    public static int ConvertToEmployeeNumber(short year, MillenniumGender gender, short sequence)
    {
        return year * 1_000_00 + (int)gender * 100_00 + sequence;
    }

    public static string ConvertToSex(MillenniumGender gender) => gender switch
    {
        MillenniumGender.Male or MillenniumGender.MMale => "남자",
        MillenniumGender.Female or MillenniumGender.MFemale => "여자",
        _ => throw new ArgumentOutOfRangeException(nameof(gender), $"예상치 못한 성별 코드: {gender}")
    };
}
