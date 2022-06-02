using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Models
{
    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }

        public UserProfile(string userName)
        {
            UserName = userName;
        }

        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }

        public UserProfile() { }
    }
}
