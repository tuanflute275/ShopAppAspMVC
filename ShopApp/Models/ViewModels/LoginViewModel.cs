using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên tài khoản không được trống")]
        [MinLength(5, ErrorMessage = "Tối thiếu 6 kí tự")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
