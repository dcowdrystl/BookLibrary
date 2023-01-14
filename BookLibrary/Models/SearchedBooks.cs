using System.ComponentModel.DataAnnotations;

namespace BookLibrary.Models
{
    
    public class SearchedBooks
    {
        [Key] public int Id { get; set; }
        public string APIBookID { get; set; }
        public string BookTitle { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string Genre { get; set; }
        public int NumberOfPages { get; set; }
        public string Image { get; set; }
        public ApplicationUser User { get; set; }
        public string ApplicationUserId { get; set; }

        public SearchedBooks()
        {
        }

        public SearchedBooks(string apibookid, string booktitle, string authorfirstname, string authorlastname, string genre, int numberofpages, string image)
        {
            APIBookID = apibookid;
            BookTitle = booktitle;
            AuthorFirstName = authorfirstname;
            AuthorLastName = authorlastname;
            Genre = genre;
            NumberOfPages = numberofpages;
            Image = image;
            
        }

    }
}
