using Microsoft.AspNetCore.Identity;

namespace SchoolManagementSystem.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? ProfilePicture { get; set; }
        public List<Message>? SendMessages { get; set; }
        public List<Message>? ReceiveMessages { get; set; }

        public string UserType { get; set; } = string.Empty; 

        public string? Class { get; set; }
        public string? Section { get; set; }
        public int? RollNumber { get; set; }

      
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }

    }
    public class AppRole : IdentityRole<Guid>        //test
    {

        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? Permissions { get; set; }
    }
}
