using System.ComponentModel.DataAnnotations;

namespace Northwind.ViewModel.Auth
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username / Email wajib diisi.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi.")]
        public string Password { get; set; } = string.Empty;
    }
}