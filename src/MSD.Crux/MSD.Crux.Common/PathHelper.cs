using System;
using System.IO;

namespace MSD.Crux.Common;

public static class PathHelper
{
    /// <summary>
    /// 홈디렉토리와 상대 경로를 절대 경로로 변환.
    /// 홈 디렉토리(~)를 절대 경로로 해석하며, 상대 경로는 실행 파일 기준으로 변환
    /// </summary>
    /// <code>
    ///  ~/myapp/images -> /home/username/myapp/images
    ///  relative/path -> /app/relative/path
    /// /absolute/path/to/images -> /absolute/path/to/images
    /// </code>
    /// <param name="path">상대 경로 또는 절대 경로</param>
    /// <returns>절대 경로</returns>
    public static string ToAbsolutePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("경로는 비어있을 수 없습니다.", nameof(path));
        }

        // ~로 시작하는 경로는 홈 디렉토리로 변환
        if (path.StartsWith("~"))
        {
            string homeDirectory = Environment.GetEnvironmentVariable("HOME") ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDirectory, path.TrimStart('~', '/'));
        }

        // 상대 경로는 실행 파일 기준으로 절대 경로로 변환
        return Path.IsPathRooted(path)
                   ? path // 이미 절대 경로인 경우 그대로 반환
                   : Path.Combine(AppContext.BaseDirectory, path);
    }


    /// <summary>
    /// 설정된 경로를 읽고 절대 경로로 변환하며, 디렉토리를 생성한다.
    /// </summary>
    /// <param name="path">설정된 경로</param>
    /// <param name="defaultPath">기본값 경로</param>
    /// <returns>절대 경로</returns>
    public static string GetOrCreateDirectory(string? path, string defaultPath = "")
    {
        string resolvedPath = PathHelper.ToAbsolutePath(path ?? defaultPath);

        if (!Directory.Exists(resolvedPath))
        {
            Directory.CreateDirectory(resolvedPath);
        }

        return resolvedPath;
    }
}
