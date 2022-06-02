using System;
using System.ComponentModel.DataAnnotations;

namespace BookLibrary.ViewModels
{
   public class AddPostViewModel
   {
      [Required]
      [Display(Name = "Review")]
      public string Content { get; set; }
      public DateTime CreatedAt { get; set; } = DateTime.Now;
      public DateTime UpdatedAt { get; set; } = DateTime.Now;
   }
}
