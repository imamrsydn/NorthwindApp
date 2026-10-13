using Northwind.DbAccess.Models;
using Northwind.ViewModel.UserAccount;

namespace Northwind.Repository.Interfaces
{
    public interface IUserAccountRepository
    {
        Task<List<UserAccount>> GetAllAccountsAsync(string? username, string? email, bool? isActive);
        Task<(List<UserAccount> Items, int TotalCount)> GetPagedAsync(UserAccountFilterDto filter);
        Task<UserAccount?> GetAccountDetailAsync(string username);
        Task UpdateAsync(UserAccount account);
        Task SaveChangesAsync();
    }
}