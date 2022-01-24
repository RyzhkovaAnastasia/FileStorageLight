using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(100, ErrorMessage = "Username must be less 100 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.{6,}$)(?=.*?\w)(?=.*?[0-9]).*$",
         ErrorMessage = "Password must contain minimum 6 characters, at least one letter and one number")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation is required")]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string PasswordConfirm { get; set; }
    }
}
