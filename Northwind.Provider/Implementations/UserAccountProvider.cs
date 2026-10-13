using Northwind.Provider.Interfaces;
using Northwind.Repository.Interfaces;
using Northwind.ViewModel;
using Northwind.ViewModel.Auth;
using Northwind.ViewModel.UserAccount;

namespace Northwind.Provider.Implementations
{
    public class UserAccountProvider : IUserAccountProvider
    {
        private readonly IUserAccountRepository _repo;
        private readonly IEmailService _emailService;

        public UserAccountProvider(IUserAccountRepository repo, IEmailService emailService)
        {
            _repo = repo;
            _emailService = emailService;
        }

        public async Task<ApiResponse<List<UserAccountListDto>>> GetAllAccountsAsync(string? username, string? email, bool? isActive)
        {
            var data = await _repo.GetAllAccountsAsync(username, email, isActive);
            var res = data.Select(u => new UserAccountListDto
            {
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CountLog = u.CountLog,
                CreatedAt = u.CreatedAt
            }).ToList();

            return ApiResponse<List<UserAccountListDto>>.SuccessResult(res);
        }

        public async Task<ApiResponse<PagedResultDto<UserAccountListDto>>> GetPagedAccountsAsync(UserAccountFilterDto filter)
        {
            var (items, totalCount) = await _repo.GetPagedAsync(filter);
            var dtos = items.Select(u => new UserAccountListDto
            {
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CountLog = u.CountLog,
                CreatedAt = u.CreatedAt
            }).ToList();

            var result = new PagedResultDto<UserAccountListDto>(dtos, totalCount, filter.PageIndex, filter.PageSize);
            return ApiResponse<PagedResultDto<UserAccountListDto>>.SuccessResult(result);
        }

        public async Task<ApiResponse<UserAccountDetailDto>> GetAccountDetailAsync(string username)
        {
            var u = await _repo.GetAccountDetailAsync(username);
            if (u == null) return ApiResponse<UserAccountDetailDto>.FailureResult("Akun tidak ditemukan.");

            var res = new UserAccountDetailDto
            {
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                CountLog = u.CountLog,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                EmployeeId = u.EmployeeId,
                EmployeeName = u.Employee != null ? $"{u.Employee.FirstName} {u.Employee.LastName}" : null,
                CustomerId = u.CustomerId,
                CustomerName = u.Customer?.CompanyName
            };

            return ApiResponse<UserAccountDetailDto>.SuccessResult(res);
        }

        public async Task<ApiResponse<bool>> ToggleUserActiveStatusAsync(UpdateUserStatusDto req)
        {
            var u = await _repo.GetAccountDetailAsync(req.Username);
            if (u == null) return ApiResponse<bool>.FailureResult("Akun tidak ditemukan.");

            u.IsActive = req.IsActive;
            u.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(u);
            await _repo.SaveChangesAsync();

            string status = req.IsActive ? "diaktifkan" : "dinonaktifkan";
            return ApiResponse<bool>.SuccessResult(true, $"Akun {u.Username} berhasil {status}.");
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(string username)
        {
            var u = await _repo.GetAccountDetailAsync(username);
            if (u == null) return ApiResponse<bool>.FailureResult("Akun tidak ditemukan.");

            const string defaultPassword = "Northwind1234567890";

            // Hash password default & reset countLog
            u.Password = BCrypt.Net.BCrypt.HashPassword(defaultPassword);
            u.CountLog = 0;
            u.IsActive = true; // Otomatis aktifkan kembali jika sebelumnya terblokir
            u.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(u);
            await _repo.SaveChangesAsync();

            // Kirim notifikasi via email jika email tersedia
            if (!string.IsNullOrWhiteSpace(u.Email))
            {
                try
                {
                    string emailBody = $@"
                        <h3>Pemberitahuan Reset Password</h3>
                        <p>Halo <b>{u.Username}</b>,</p>
                        <p>Password akun Anda telah di-reset oleh Admin.</p>
                        <p>Berikut rincian kredensial baru Anda:</p>
                        <ul>
                            <li><b>Username:</b> {u.Username}</li>
                            <li><b>Password Default:</b> {defaultPassword}</li>
                        </ul>
                        <p>Segera lakukan login dan ubah password Anda demi keamanan akun.</p>
                        <br/>
                        <p>Salam,<br/><b>Northwind API Admin</b></p>";

                    await _emailService.SendEmailAsync(u.Email, "Reset Password Akun Northwind API", emailBody);
                }
                catch (Exception ex)
                {
                    return ApiResponse<bool>.SuccessResult(true, $"Password akun {username} berhasil di-reset ke default, namun email gagal dikirim: {ex.Message}");
                }
            }

            return ApiResponse<bool>.SuccessResult(true, $"Password akun {username} berhasil di-reset ke default dan email notifikasi telah dikirim.");
        }
    }
}