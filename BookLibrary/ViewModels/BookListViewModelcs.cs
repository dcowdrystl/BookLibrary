using BookLibrary.Models;
using System.Collections.Generic;

namespace BookLibrary.ViewModels
{
    public class BookListViewModel
    {
        public List<Book> Books { get; set; }
        public int TotalBooks { get; set; }
        public int MyW2RBooks { get; set; }
        public int MyReadingBooks { get; set; }
        public int MyReadBooks { get; set; }
        public List<string> BookTitlesW2R { get; set; }
        public List<string> BookTitlesReading { get; set; }
        public List<string> BookTitlesRead { get; set; }
    }
}