using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniCinema.DTOs
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public IList<string> Roles { get; set; } = new List<string>();
        public bool IsLockedOut { get; set; }
    }

    public class UserCreateDto
    {
        [Required(ErrorMessage = "Tài khoản không được trống")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được trống")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên không được trống")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Email không được trống")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Số điện thoại không được trống")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Role { get; set; } = "Staff";
    }

    public class UserEditDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên không được trống")]
        public string HoTen { get; set; } = null!;

        [Required(ErrorMessage = "Email không được trống")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;
        
        // Mật khẩu mới nếu muốn đổi
        public string? NewPassword { get; set; }
    }
}
