using Microsoft.Extensions.DependencyInjection;
using MSD.Crux.Core.IRepositories;
using MSD.Crux.Core.IServices;
using MSD.Crux.Infra.Repositories;
using MSD.Crux.Infra.Services;

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
        services.AddScoped<IVisionNgService, VisionNgService>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IInjectionPlanService, InjectionPlanService>();
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
        services.AddTransient<IPartRepo, PartRepoPsqlDb>();
        services.AddTransient<IVisionCumRepo, VisionCumRepoPsqlDb>();
        services.AddTransient<IInjectionCumRepo, InjectionCumRepoPsqlDb>();
        services.AddTransient<ILotRepo, LotRepoPsqlDb>();
        services.AddTransient<IVisionNgRepo, VisionNgRepoPsqlDb>();
        services.AddTransient<IInjectionPlanRepo, InjectionPlanRepoPsqlDb>();
        return services;
    }
}
