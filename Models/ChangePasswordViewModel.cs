
using System.ComponentModel.DataAnnotations;
public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "كلمة السر الحالية مطلوبة.")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة السر الحالية")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "كلمة السر الجديدة مطلوبة.")]
    [DataType(DataType.Password)]
    [Display(Name = "كلمة السر الجديدة")]
    [MinLength(6, ErrorMessage = "كلمة السر الجديدة يجب أن تحتوي على 6 أحرف على الأقل.")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "تأكيد كلمة السر الجديدة")]
    [Compare("NewPassword", ErrorMessage = "كلمتا السر غير متطابقتين.")]
    public string ConfirmPassword { get; set; }
}