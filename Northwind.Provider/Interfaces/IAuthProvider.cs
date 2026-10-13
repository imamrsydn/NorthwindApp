using Northwind.ViewModel.Auth;

namespace Northwind.Provider.Interfaces;

public interface IAuthProvider
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<UserProfileDto>> GetProfileAsync(string username);
    Task<ApiResponse<bool>> ChangePasswordAsync(string username, ChangePasswordDto request);
}