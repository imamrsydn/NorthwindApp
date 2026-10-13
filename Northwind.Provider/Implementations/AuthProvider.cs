using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Northwind.DbAccess.Models;
using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Northwind.Provider.Implementations;

public class AuthProvider : IAuthProvider
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthProvider(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username)
                   ?? await _userRepository.GetByEmailAsync(request.Username);

        if (user == null)
        {
            return ApiResponse<LoginResponseDto>.FailureResult("Username/Email atau password salah.");
        }

        if (!user.IsActive)
        {
            return ApiResponse<LoginResponseDto>.FailureResult("Akun Anda terblokir/tidak aktif. Silakan hubungi Admin.");
        }

        if (!VerifyPassword(request.Password, user.Password))
        {
            user.CountLog += 1;
            user.UpdatedAt = DateTime.UtcNow;

            if (user.CountLog > 3)
            {
                user.IsActive = false;
                await _userRepository.SaveChangesAsync();
                return ApiResponse<LoginResponseDto>.FailureResult("Akun Anda telah dinonaktifkan karena salah password lebih dari 3 kali. Hubungi Admin.");
            }

            await _userRepository.SaveChangesAsync();
            int remainingAttempts = 3 - user.CountLog + 1;
            return ApiResponse<LoginResponseDto>.FailureResult($"Password salah. Sisa percoban: {remainingAttempts} kali.");
        }

        user.CountLog = 0;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();

        var token = GenerateJwtToken(user, out DateTime expiresAt);

        var response = new LoginResponseDto
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
            User = MapToProfileDto(user)
        };

        return ApiResponse<LoginResponseDto>.SuccessResult(response, "Login berhasil.");
    }

    public async Task<ApiResponse<UserProfileDto>> GetProfileAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return ApiResponse<UserProfileDto>.FailureResult("User tidak ditemukan.");

        return ApiResponse<UserProfileDto>.SuccessResult(MapToProfileDto(user));
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(string username, ChangePasswordDto request)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return ApiResponse<bool>.FailureResult("User tidak ditemukan.");

        if (!VerifyPassword(request.OldPassword, user.Password))
        {
            return ApiResponse<bool>.FailureResult("Password lama salah.");
        }

        user.Password = HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();

        return ApiResponse<bool>.SuccessResult(true, "Password berhasil diperbarui.");
    }

    #region Helpers
    private string GenerateJwtToken(UserAccount user, out DateTime expiresAt)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

        expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"] ?? "120"));

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        if (!string.IsNullOrEmpty(user.Email))
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

        if (user.EmployeeId.HasValue)
            claims.Add(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));

        if (!string.IsNullOrEmpty(user.CustomerId))
            claims.Add(new Claim("CustomerId", user.CustomerId));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    private static UserProfileDto MapToProfileDto(UserAccount user) => new()
    {
        Username = user.Username,
        Email = user.Email,
        Role = user.Role,
        EmployeeId = user.EmployeeId,
        CustomerId = user.CustomerId,
        IsActive = user.IsActive,
        CountLog = user.CountLog,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };

    private static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    private static bool VerifyPassword(string password, string hash)
    {
        // fallback kalau password di DB masih plaintext
        if (!hash.StartsWith("$2a$") && !hash.StartsWith("$2b$") && !hash.StartsWith("$2y$"))
            return password == hash;

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
    #endregion
}