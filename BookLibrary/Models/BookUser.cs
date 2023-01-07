using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Models
{
    public class BookUser
    {
        //public int Id { get; set; }

        public int BookId { get; set; }
      public string ApiBookID { get; set; }
        public Book Book { get; set; }

        ////[Key]
        //public string RUserId { get; set; }

        //[ForeignKey("ApplicationUserId")]
        public ApplicationUser User { get; set; }
        public string ApplicationUserId { get; set; }
        public List<Post> Posts { get; set; }

      /*public List<ApplicationUser> Friends { get; set; } = new List<ApplicationUser>();*/
      public BookUser()
        {

        }

        
    }
}
