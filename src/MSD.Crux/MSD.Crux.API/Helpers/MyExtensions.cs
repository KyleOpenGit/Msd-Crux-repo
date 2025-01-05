using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MSD.Crux.API.Helpers;

/// <summary>
/// 커스텀 확장 메서드 모음
/// </summary>
public static class MyExtensions
{
    public static bool IsLocal(this IHostEnvironment hostEnvironment) => hostEnvironment.IsEnvironment("Local");
}
