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
        services.AddScoped<IProductService, ProductService>();

        // 4. đăng ký cho OptionGroup Service
        services.AddScoped<IOptionGroupService, OptionGroupService>();

        //5. Đăng ký cho Option Service
        services.AddScoped<IOptionService, OptionService>();

        //6. Đăng ký cho Table Service
        services.AddScoped<ITableService, TableService>();

        //7. đăng ký cho wallet service
        services.AddScoped<IWalletService, WalletService>();
        
        //8. Đăng ký cho Order Service
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}