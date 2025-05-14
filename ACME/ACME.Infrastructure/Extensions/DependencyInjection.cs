using ACME.Application.Interfaces;
using ACME.Application.Services;
using ACME.Infrastructure.Persistence;
using ACME.Infrastructure.Repositories;
using ACME.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ACME.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Registra repositorios
        services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
        services.AddSingleton<ICourseRepository, InMemoryCourseRepository>();

        // Registra servicios de infraestructura
        services.AddSingleton<IPaymentGateway, DummyPaymentGateway>();
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra servicios de aplicación
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ICourseService, CourseService>();

        return services;
    }
}