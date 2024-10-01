using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên tài khoản không được trống")]
        [MinLength(3, ErrorMessage = "Tối thiếu 3 kí tự")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 kí tự")]
        [DataType(DataType.Password)]
        public string? UserPassword { get; set; }
    }
}
