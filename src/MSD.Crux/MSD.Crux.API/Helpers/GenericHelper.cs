namespace MSD.Crux.API.Helpers;

public static class GenericHelper
{
    /// <summary>
    /// Employee 테이블의 Year, Gender, Sequence를 기반으로 고유한 Employee Number를 생성
    /// </summary>
    /// <param name="year">입사 연도</param>
    /// <param name="gender">성별 코드</param>
    /// <param name="sequence">입사 순번</param>
    /// <returns>조합된 Employee Number: 연도(4자리) + 성별(1자리) + 순번(4자리)</returns>
    public static int ConvertToEmployeeNumber(short year, short gender, short sequence) => year * 1000000 + gender * 10000 + sequence;
}
