using Microsoft.Extensions.DependencyInjection;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Application.Services;

namespace SmartCafe.API.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 1. Đăng ký Auth Service
        services.AddScoped<IAuthService, AuthService>();

        // 2. Đăng ký Category Service 
        services.AddScoped<ICategoryService, CategoryService>();

        // 3. Đăng ký Product Service 
        // services.AddScoped<IProductService, ProductService>();

        return services;
    }
}