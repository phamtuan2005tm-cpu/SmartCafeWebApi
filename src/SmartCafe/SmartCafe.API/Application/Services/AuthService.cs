using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartCafe.API.Application.Interfaces;
using SmartCafe.API.Domain.DTOs.Auth;
using SmartCafe.API.Domain.Entities;
using SmartCafe.API.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartCafe.API.Application.Services;

public class AuthService : IAuthService
{
    private readonly SmartCafeDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(SmartCafeDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return null;

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (existingUser)
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = HashPassword(request.Password),
            Role = "Customer",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var refreshTokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (refreshTokenEntity == null ||
            refreshTokenEntity.IsRevoked ||
            refreshTokenEntity.ExpiresAt <= DateTime.UtcNow ||
            !refreshTokenEntity.User.IsActive)
        {
            return null;
        }

        // Vô hiệu hóa Token cũ (Revoke)
        refreshTokenEntity.IsRevoked = true;
        _context.RefreshTokens.Update(refreshTokenEntity);

        // Sinh cặp Token mới
        return await GenerateAuthResponseAsync(refreshTokenEntity.User);
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var tokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity == null || tokenEntity.IsRevoked)
            return false;

        tokenEntity.IsRevoked = true;
        await _context.SaveChangesAsync();
        return true;
    }

    // =========================================================================
    // HELPER METHODS
    // =========================================================================

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        var jwtSettings = _configuration.GetSection("JwtSettings");
        var refreshTokenExpiryDays = int.Parse(jwtSettings["RefreshTokenExpirationDays"] ?? "7");
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = refreshTokenExpiresAt,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        };
    }

    private string GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryMinutes = double.Parse(jwtSettings["AccessTokenExpirationMinutes"] ?? "15");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FullName", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string inputPassword, string storedHash)
    {
        var hashOfInput = HashPassword(inputPassword);
        return hashOfInput == storedHash;
    }
}