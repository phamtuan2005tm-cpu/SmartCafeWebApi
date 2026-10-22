using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Application.Services;
using SmartCafe.API.Infrastructure.Data;
using System.Text;

namespace SmartCafe.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // =========================================================================
        // 1. ĐĂNG KÝ CÁC DỊCH VỤ CƠ BẢN VÀ CONTROLLERS
        // =========================================================================
        // Đăng ký dịch vụ Controller để ASP.NET nhận diện các API Controller
        builder.Services.AddControllers();

        // Đăng ký Explorer giúp Swagger quét các API Endpoints
        builder.Services.AddEndpointsApiExplorer();

        // =========================================================================
        // 2. CẤU HÌNH SWAGGER KÈM NÚT "AUTHORIZE" (Ổ KHÓA TEST JWT)
        // =========================================================================
        // Cấu hình Swagger UI để hiển thị tài liệu API và tích hợp ô nhập Token Bearer
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SmartCafe API",
                Version = "v1",
                Description = "Hệ thống API Quản lý Quán Cafe - SmartCafe"
            });

            // Định nghĩa Security Scheme theo chuẩn JWT Bearer
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Nhập Token theo cú pháp: Bearer {chuỗi_access_token}"
            });

            // Áp dụng yêu cầu Security này cho tất cả các Endpoints trên giao diện Swagger
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // =========================================================================
        // 3. ĐĂNG KÝ DBCONTEXT (POSTGRESQL) & CÁC SERVICES
        // =========================================================================
        // Khởi tạo DbContext kết nối đến CSDL PostgreSQL thông qua ConnectionString
        builder.Services.AddDbContext<SmartCafeDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Đăng ký AuthService với vòng đời Scoped (tạo instance mới cho mỗi HTTP Request)
        builder.Services.AddScoped<IAuthService, AuthService>();

        // =========================================================================
        // 4. CẤU HÌNH JWT AUTHENTICATION (XÁC THỰC NGƯỜI DÙNG)
        // =========================================================================
        // Đọc cấu hình JwtSettings từ appsettings.json
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;

        // Đăng ký dịch vụ Xác thực Authentication sử dụng Bearer Token
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            // Thiết lập các tham số kiểm tra tính hợp lệ của Token gửi lên
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,             // Kiểm tra Publisher của Token
                ValidateAudience = true,           // Kiểm tra Đối tượng sử dụng Token
                ValidateLifetime = true,           // Kiểm tra Token còn hạn hay đã hết hạn
                ValidateIssuerSigningKey = true,   // Kiểm tra Chữ ký bảo mật với SecretKey
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ClockSkew = TimeSpan.Zero          // Bắt buộc Token hết hạn chính xác từng giây
            };
        });

        // Build ứng dụng sau khi hoàn tất đăng ký toàn bộ Service vào DI Container
        var app = builder.Build();

        // =========================================================================
        // 5. CẤU HÌNH HTTP REQUEST PIPELINE (MIDDLEWARES)
        // =========================================================================
        // Kích hoạt giao diện Swagger UI ở môi trường Development
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartCafe API v1");
            });
        }

        // Tự động chuyển hướng các request HTTP sang HTTPS
        app.UseHttpsRedirection();

        // ⚠️ VỊ TRÍ BẮT BUỘC: UseAuthentication phải đứng TRƯỚC UseAuthorization
        // Giúp giải mã JWT Token và trích xuất thông tin User (Claims)
        app.UseAuthentication();

        // Giúp kiểm tra quyền truy cập (Role Admin/Staff/Customer) vào từng API
        app.UseAuthorization();

        // Bản đồ hóa các đường dẫn Controller vào hệ thống Routing
        app.MapControllers();

        // Lệnh kích hoạt Server bắt đầu lắng nghe các Request
        app.Run();
    }
}