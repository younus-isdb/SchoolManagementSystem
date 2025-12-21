using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username or Email is required")]
        [Display(Name = "Username or Email")]
        public string UserNameOrEmail { get; set; } = string.Empty;


        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6)]
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; } = default!;

        public bool RememberMe { get; set; }

        [Required]
        public string UserType { get; set; } // "Admin", "Student", "Teacher", etc.

        public string? Class { get; set; } // For students
        public string? Section { get; set; } // For students
        public int? RollNumber { get; set; } // For students

        public string ReturnUrl { get; set; } = "/";


    }
}
