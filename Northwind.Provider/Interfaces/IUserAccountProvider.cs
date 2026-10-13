using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.UserAccount;

namespace Northwind.Provider.Interfaces
{
    public interface IUserAccountProvider
    {
        Task<ApiResponse<List<UserAccountListDto>>> GetAllAccountsAsync(string? username, string? email, bool? isActive);
        Task<ApiResponse<PagedResultDto<UserAccountListDto>>> GetPagedAccountsAsync(UserAccountFilterDto filter);
        Task<ApiResponse<UserAccountDetailDto>> GetAccountDetailAsync(string username);
        Task<ApiResponse<bool>> ToggleUserActiveStatusAsync(UpdateUserStatusDto req);
        Task<ApiResponse<bool>> ResetPasswordAsync(string username);
    }
}