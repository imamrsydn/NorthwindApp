using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel.UserAccount;

namespace Northwind.Repository.Implementations
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly Northwind3Context _context;

        public UserAccountRepository(Northwind3Context context) { _context = context; }

        public async Task<List<UserAccount>> GetAllAccountsAsync(string? username, string? email, bool? isActive)
        {
            var q = _context.UserAccounts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(username))
                q = q.Where(u => u.Username.Contains(username));

            if (!string.IsNullOrWhiteSpace(email))
                q = q.Where(u => u.Email != null && u.Email.Contains(email));

            if (isActive.HasValue)
                q = q.Where(u => u.IsActive == isActive.Value);

            return await q.ToListAsync();
        }

        public async Task<(List<UserAccount> Items, int TotalCount)> GetPagedAsync(UserAccountFilterDto filter)
        {
            var query = _context.UserAccounts.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.SearchKeyword))
            {
                var keyword = filter.SearchKeyword.Trim().ToLower();
                query = query.Where(u => u.Username.ToLower().Contains(keyword) ||
                                         (u.Email != null && u.Email.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Username))
                query = query.Where(u => u.Username.Contains(filter.Username));

            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(u => u.Email != null && u.Email.Contains(filter.Email));

            if (!string.IsNullOrWhiteSpace(filter.Role))
                query = query.Where(u => u.Role == filter.Role);

            if (filter.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filter.IsActive.Value);

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.Username)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<UserAccount?> GetAccountDetailAsync(string username)
        {
            return await _context.UserAccounts
                .Include(u => u.Employee)
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task UpdateAsync(UserAccount account)
        {
            _context.UserAccounts.Update(account);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}