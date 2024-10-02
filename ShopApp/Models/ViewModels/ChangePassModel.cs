using System.ComponentModel.DataAnnotations;

namespace ShopApp.Models.ViewModels
{
    public class ChangePassModel
    {
        [Required(ErrorMessage = "Không được bỏ trống")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 kí tự")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Không được bỏ trống")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 kí tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Không được bỏ trống")]
        [MinLength(6, ErrorMessage = "Tối thiểu 6 kí tự")]
        public string CFPassword { get; set; }


    }
}
