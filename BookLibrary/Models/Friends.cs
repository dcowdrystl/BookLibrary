using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Models
{
    public class Friends
    {
        [Key]
        public int RelationId { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserProfile User { get; set; }

        public int FriendId { get; set; }

       [ForeignKey("FriendId")]
        public UserProfile Friend { get; set; }

        public Friends(int userId, int friendId)
        {
            UserId = userId;
            FriendId = friendId;
        }

        public Friends() { }
    }
}
