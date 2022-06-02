using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        [Display(Name = "Review")]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

      // Navigation and foreign keys
      public string UserId { get; set; }
      public ApplicationUser User { get; set; }

      public string ApplicationUserId { get; set; }
 
      public int BookId { get; set; }
        public Book PostBook{ get; set; }

      /*public List<Like> Likes { get; set; }
      public List<Comment> Comments { get; set; }*/
    }
}