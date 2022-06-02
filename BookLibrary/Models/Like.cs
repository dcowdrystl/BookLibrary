using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrary.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation and foreign keys
        public int PostId { get; set; }
        public Post LikePost { get; set; }

        public string UserId { get; set; }
       public string ApplicationUserId { get; set; }
   ///   public ApplicationUser ApplicationUser { get; set; }
      public ApplicationUser LikeUser { get; set; }
    }
}