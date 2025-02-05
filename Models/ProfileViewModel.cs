

using System.ComponentModel.DataAnnotations;

public class ProfileViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string UserName { get; set; }

    [Required]
    [Phone]
    public string ?PhoneNumber { get; set; }

   


}
    



