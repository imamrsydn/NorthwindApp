using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Auth
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Password lama wajib diisi.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password baru wajib diisi.")]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter.")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Konfirmasi password tidak cocok.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}