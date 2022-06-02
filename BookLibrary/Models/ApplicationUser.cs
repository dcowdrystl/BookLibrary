using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookLibrary.Models
{
    public class ApplicationUser : IdentityUser
    {

        public IList<BookUser> BookUsers { get; set; }

        [PersonalData]
        [Column(TypeName ="nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }
      public List<Post> Posts { get; set; }
      public ApplicationUser()
        {

        }
    }

    
}
