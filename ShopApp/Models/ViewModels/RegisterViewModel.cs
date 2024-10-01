using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage = "Tên tài khoản không được bỏ trống")]
        [MinLength(3, ErrorMessage="Tối thiểu 3 kí tự")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 kí tự")]
        public string? UserPassword { get; set; }

        [Display(Name = "Tên đầy đủ")]
        [Required(ErrorMessage = "Tên đầy đủ không được bỏ trống")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string? UserFullName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string? UserEmail { get; set; }

    }
}
