using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSD.Crux.Common;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Infra.Repositories;
using MSD.Crux.Infra.Services;
using Npgsql;

namespace MSD.Crux.Shared;

/// <summary>
/// 여러 프로젝트의 공통적으로 호출할 수 있는 IServiceCollection 확장 메서드를 모은 클래스
/// </summary>
public static class DiRegistrar
{
    /// <summary>
    /// Crux 서버의 서비스 계층(Service Layer)의 모든 인터페이스와 구현체를 의존성 주입(DI) 컨테이너에 등록해준다.
    /// </summary>
    /// <param name="services">의존성이 등록될, DI 컨테이너를 구성하는 <see cref="IServiceCollection"/> 인스턴스</param>
    /// <returns>의존성이 등록된 <see cref="IServiceCollection"/> 인스턴스</returns>
    public static IServiceCollection AddCruxServicesAll(this IServiceCollection services)
    {
        //Crux 서비스 계층 등록
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserLoginService, UserLoginService>();
        services.AddScoped<ILotService, LotService>();
        return services;
    }

    /// <summary>
    /// Crux 서버의 레포지토리 계층(Repository Layer)의 모든 인터페이스와 구현체를 의존성 주입(DI) 컨테이너에 등록해준다.
    /// In-memory 구현체 또는 DB 구현체를 등록해준다.
    /// </summary>
    /// <param name="services">의존성이 등록될, DI 컨테이너를 구성하는 <see cref="IServiceCollection"/> 인스턴스</param>
    /// <returns>의존성이 등록된 <see cref="IServiceCollection"/> 인스턴스</returns>
    public static IServiceCollection AddCruxRepositoriesAll(this IServiceCollection services)
    {
        // Crux 레포지토리 계층 등록
        services.AddTransient<IEmployeeRepo, EmployeeRepoPsqlDb>();
        services.AddTransient<IUserRepo, UserRepoPsqlDb>();
        services.AddTransient<IVisionCumRepo, VisionCumRepoPsqlDb>();
        services.AddTransient<ILotRepo, LotRepoPsqlDb>();
        return services;
    }

    /// <summary>
    /// 공통 구성 설정을 등록한다.
    /// </summary>
    /// <param name="services">의존성이 등록될, DI 컨테이너를 구성하는 <see cref="IServiceCollection"/> 인스턴스</param>
    /// <param name="configuration">구성 파일 객체</param>
    /// <returns>의존성이 등록된 <see cref="IServiceCollection"/> 인스턴스</returns>
    public static IServiceCollection AddCruxSharedConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Dapper 기본 설정
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        // Npgsql DB 커넥션 스트링 객체
        services.AddTransient<IDbConnection>(sp =>
                                             {
                                                 string? connectionString = configuration.GetConnectionString("Postgres");
                                                 return new NpgsqlConnection(connectionString);
                                             });

        // JWT 인증 관련 설정
        services.AddAuthentication("Bearer").AddJwtBearer(options =>
                                                          {
                                                              options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                                                              {
                                                                  ValidateIssuer = true,
                                                                  ValidateAudience = true,
                                                                  ValidateLifetime = true,
                                                                  ValidateIssuerSigningKey = true,
                                                                  ValidIssuer = configuration["Jwt:Issuer"],
                                                                  ValidAudience = configuration["Jwt:Audience"],
                                                                  IssuerSigningKey = JwtHelper.GetPublicKey(configuration)
                                                              };
                                                          });


        // 로깅 설정 추가
        services.AddLogging(logging =>
                            {
                                logging.AddConsole();
                                logging.AddDebug();
                            });


        return services;
    }
}
