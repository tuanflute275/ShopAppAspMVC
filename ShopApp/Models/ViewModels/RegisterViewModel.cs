using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Tên tài khoản")]
        [Required(ErrorMessage = "*")]
        [MinLength(3, ErrorMessage="Tối thiểu 3 kí tự")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 kí tự")]
        public string UserPassword { get; set; }

        [Display(Name = "Tên đầy đủ")]
        [Required(ErrorMessage = "*")]
        [MaxLength(100, ErrorMessage = "Tối đa 100 kí tự")]
        public string UserFullName { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string UserEmail { get; set; }

        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? UserPhone { get; set; }

        [Display(Name = "Giới tính")]
        public string? UserGender { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? UserAddress { get; set; }
    }
}
