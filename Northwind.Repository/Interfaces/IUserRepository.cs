using Northwind.DbAccess.Models;

namespace Northwind.Repository.Interfaces;

public interface IUserRepository
{
    Task<UserAccount?> GetByUsernameAsync(string username);
    Task<UserAccount?> GetByEmailAsync(string email);
    Task UpdateAsync(UserAccount user);
    Task SaveChangesAsync();
}