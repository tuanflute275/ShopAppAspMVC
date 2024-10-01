using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class ForgotPasswordModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string? UserEmail { get; set; }
    }
}
