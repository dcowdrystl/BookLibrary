using System.Collections.Generic;

namespace BookLibrary.Models
{
    public class BookUser
    {
        public int BookId { get; set; }
        public string ApiBookID { get; set; }
        public bool isFavorite { get; set; }
        public bool isWantToRead { get; set; }
        public bool isRead { get; set; }
        public bool isReading { get; set; }
        public Book Book { get; set; }
        public ApplicationUser User { get; set; }
        public string ApplicationUserId { get; set; }
        public List<Post> Posts { get; set; }

        /*public List<ApplicationUser> Friends { get; set; } = new List<ApplicationUser>();*/
        public BookUser()
        {

        }


    }
}
