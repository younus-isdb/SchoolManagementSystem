using Microsoft.AspNetCore.Identity;

namespace SchoolManagementSystem.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? ProfilePicture { get; set; }
        public List<Message>? SendMessages { get; set; }
        public List<Message>? ReceiveMessages { get; set; }

	}
    public class AppRole : IdentityRole<Guid>        //test
    {

        public string? Description { get; set; }
    }
}
