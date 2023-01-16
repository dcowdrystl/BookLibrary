using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookLibrary.Models
{
    public class Book
    {
        public string BookTitle { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string Genre { get; set; }
        public int NumberOfPages { get; set; }
        public string Image { get; set; } = "no image";
        public int Id { get; set; }
      public string APIBookID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string SearchInfo { get; set; }



        //[ForeignKey("ApplicationUserId")]
        public ApplicationUser User { get; set; }

        //[Key]
        public string ApplicationUserId { get; set; }

        public IList<BookUser> BookUsers { get; set; }
        public List<Post> Posts { get; set; }

      public Book()
        {
        }

        public Book(string booktitle, string authorfirstname, string authorlastname, string genre, int numberofpages, string image, string apiBookID, string searchinfo)
        {
            BookTitle = booktitle;
            AuthorFirstName = authorfirstname;
            AuthorLastName = authorlastname;
            Genre = genre;
            NumberOfPages = numberofpages;
            Image = image;
            APIBookID = apiBookID;
            CreatedAt = DateTime.Now;
            SearchInfo = searchinfo;
        }      

        public override string ToString()
        {
            return BookTitle;
        }

        public override bool Equals(object obj)
        {
            return obj is Book book &&
                   Id == book.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
