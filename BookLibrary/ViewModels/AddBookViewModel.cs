using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.ViewModels
{
    public class AddBookViewModel
    {
        [Required(ErrorMessage = "You must include a Title")]
        public string BookTitle { get; set; }
        public string AuthorFirstName { get; set; }
        [Required(ErrorMessage = "You must include the authors last name")]
        public string AuthorLastName { get; set; }
        public string Genre { get; set; }
        public int NumberOfPages { get; set; }
      
      //[Required]
      [Display(Name = "Image URL")]
      public string Image { get; set; }
   }
}
