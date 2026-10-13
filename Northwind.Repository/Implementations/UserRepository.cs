using Microsoft.EntityFrameworkCore;
using Northwind.DbAccess.Models;
using Northwind.Repository.Interfaces;

namespace Northwind.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly Northwind3Context _context;

    public UserRepository(Northwind3Context context)
    {
        _context = context;
    }

    public async Task<UserAccount?> GetByUsernameAsync(string username)
    {
        return await _context.UserAccounts
            .Include(u => u.Employee)
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<UserAccount?> GetByEmailAsync(string email)
    {
        return await _context.UserAccounts
            .Include(u => u.Employee)
            .Include(u => u.Customer)
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());
    }

    public async Task UpdateAsync(UserAccount user)
    {
        _context.UserAccounts.Update(user);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}