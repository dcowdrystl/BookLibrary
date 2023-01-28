using BookLibrary.Data;
using BookLibrary.Models;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//AIzaSyACdHSQbarZ_V5SzuDEg8UQUQQX_tKfpvA
namespace BookLibrary.Controllers
{
    [Authorize]
    public class ListController : Controller
    {
        private static string API_KEY = "AIzaSyACdHSQbarZ_V5SzuDEg8UQUQQX_tKfpvA";

        public static BooksService service = new BooksService(new BaseClientService.Initializer
        {
            ApplicationName = "BookSearch",
            ApiKey = API_KEY,
        });

        private BookDbContext context;
        private UserManager<ApplicationUser> _userManager;
        public ListController(BookDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            context = dbContext;
            _userManager = userManager;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        public async Task<IActionResult> IndexAsync(string searchTerm)
        {
            List<SearchedBooks> books = context.SearchedBooks
            .ToList();

            ViewBag.allBooks = books.Count();
            ViewBag.bookTitles = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm))
            {

                books = context.SearchedBooks
                        .Where(j => j.BookTitle.Contains(searchTerm.Trim()) || j.AuthorFirstName.Contains(searchTerm.Trim())
                        || j.AuthorLastName.Contains(searchTerm.Trim()) || j.Genre.Contains(searchTerm.Trim()))
                        .ToList();
            }

            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await GetCurrentUserAsync();

                var titles = (from b in context.Books
                              join bu in context.BookUsers on b.Id equals bu.BookId
                              join au in context.ApplicationUsers on bu.ApplicationUserId equals au.Id
                              where au.Id == currentUser.Id
                              select new SearchedBooks
                              {
                                  //  Id = b.Id,
                                  BookTitle = b.BookTitle,
                                  AuthorFirstName = b.AuthorFirstName,
                                  AuthorLastName = b.AuthorLastName,
                                  Genre = b.Genre,
                                  NumberOfPages = b.NumberOfPages,
                                  // ApplicationUserId = ""
                                  APIBookID = b.APIBookID
                              }).ToList();

                foreach (var book in titles)
                {

                    ViewBag.bookTitles.Add(book.BookTitle);
                }
            }

            return View(books);
        }

        public IActionResult List()
        {
            ViewBag.books = context.Books
            .ToList();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> ShowGenre(string searchTerm, string genre)
        {
            Book book1 = new Book();

            List<Book> books = context.Books
            .Where(b => b.Genre == genre)
            .ToList();
            ViewBag.allBooks = books.Count();


            ViewBag.bookTitles = new List<string>();
            if (!string.IsNullOrEmpty(searchTerm))
            {

                books = context.Books
                        .Where(j => j.BookTitle.Contains(searchTerm.Trim()) || j.AuthorFirstName.Contains(searchTerm.Trim())
                        || j.AuthorLastName.Contains(searchTerm.Trim()) || j.Genre.Contains(searchTerm.Trim()))
                        .ToList();
            }

            if (User.Identity.IsAuthenticated)
            {
                var currentUser = await GetCurrentUserAsync();

                var titles = (from b in context.Books
                              join bu in context.BookUsers on b.Id equals bu.BookId
                              join au in context.ApplicationUsers on bu.ApplicationUserId equals au.Id
                              where au.Id == currentUser.Id
                              select new SearchedBooks
                              {
                                  // Id = b.Id,
                                  BookTitle = b.BookTitle,
                                  AuthorFirstName = b.AuthorFirstName,
                                  AuthorLastName = b.AuthorLastName,
                                  Genre = b.Genre,
                                  NumberOfPages = b.NumberOfPages,
                                  // ApplicationUserId = ""
                                  APIBookID = b.APIBookID
                              }).ToList();

                foreach (var book in titles)
                {

                    ViewBag.bookTitles.Add(book.BookTitle);
                }
            }

            return View("Index", books);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetBooksAsync(string searchTerm)
        {
            if (!ModelState.IsValid) { return View(); }
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.testing = new List<SearchedBooks>();
                    var currentUser = await GetCurrentUserAsync();
                    var queryList = service.Volumes.List(searchTerm);
                    queryList.MaxResults = 10;


                    var result = queryList.Execute();
                    ViewBag.result = result;

                    foreach (var resultItem in result.Items)
                    {

                        if (result != null)
                        {
                            var booksApi = result.Items
                                .Where(b => b.VolumeInfo.Categories != null)
                                .Where(b => b.VolumeInfo.ImageLinks != null)
                                .Where(b => b.VolumeInfo.Title != null)
                                .Where(b => b.VolumeInfo.Authors != null)
                                .Where(b => b.VolumeInfo.PageCount != null)
                                .Where(b => b.VolumeInfo.Publisher != null)
                                .Where(b => b.VolumeInfo.PublishedDate != null)
                                .Where(b => b.SearchInfo != null)
                                .Select(b => new SearchedBooks
                                {

                                    BookTitle = b.VolumeInfo.Title,
                                    // AuthorFirstName = b.VolumeInfo.Authors[0],
                                    AuthorLastName = b.VolumeInfo.Authors.FirstOrDefault(),
                                    Genre = b.VolumeInfo.Categories[0],
                                    NumberOfPages = (int)b.VolumeInfo.PageCount,
                                    ApplicationUserId = currentUser.Id,
                                    Image = b.VolumeInfo.ImageLinks.Thumbnail,
                                    APIBookID = b.Id,
                                    Publisher = b.VolumeInfo.Publisher,
                                    PublishedDate = b.VolumeInfo.PublishedDate,
                                    SearchInfo = b.SearchInfo.TextSnippet

                                })
                               .ToList();



                            ViewBag.booksApi = booksApi;
                            List<SearchedBooks> testing = new List<SearchedBooks>();
                            foreach (var testBook in booksApi)
                            {
                                if (testBook.ApplicationUserId == currentUser.Id)
                                {
                                    SearchedBooks hello = new SearchedBooks
                                    {
                                        // Id = testBook.Id,
                                        BookTitle = testBook.BookTitle,
                                        AuthorFirstName = testBook.AuthorFirstName,
                                        AuthorLastName = testBook.AuthorLastName,
                                        Genre = testBook.Genre,
                                        NumberOfPages = (int)testBook.NumberOfPages,
                                        ApplicationUserId = currentUser.Id,
                                        Image = testBook.Image,
                                        APIBookID = testBook.APIBookID,
                                        Publisher = testBook.Publisher,
                                        PublishedDate = testBook.PublishedDate,
                                        SearchInfo = testBook.SearchInfo
                                    };

                                    testing.Add(hello);

                                    Book abc = new Book
                                    {
                                        BookTitle = testBook.BookTitle,
                                        AuthorFirstName = testBook.AuthorFirstName,
                                        AuthorLastName = testBook.AuthorLastName,
                                        Genre = testBook.Genre,
                                        NumberOfPages = (int)testBook.NumberOfPages,
                                        ApplicationUserId = currentUser.Id,
                                        Image = testBook.Image,
                                        APIBookID = testBook.APIBookID,
                                        Publisher = testBook.Publisher,
                                        PublishedDate = testBook.PublishedDate,
                                        SearchInfo = testBook.SearchInfo
                                    };

                                    context.SearchedBooks.Add(hello);
                                    await context.SaveChangesAsync();
                                }
                            }
                            ViewBag.testingBooks = testing;
                            return View("SearchedBooks", testing);

                        }

                        else
                        {
                            return null;
                        }

                    }
                }
                catch { return Redirect("/Home"); }

            }
            return View();
        }
        // [Route("List/GetBooks")]
        [HttpGet]
        [Route("List/BookDetails/{bookId}")]
        public IActionResult Add(string bookId)
        {
            SearchedBooks addingBook = new SearchedBooks();
            addingBook = context.SearchedBooks
                   .FirstOrDefault(b => b.APIBookID == bookId);
            foreach (var book in context.SearchedBooks)
            {
                if (book.APIBookID == bookId)
                {
                    addingBook = book;

                }
            }

            ViewBag.bookToAdd = addingBook;
            ViewBag.title = "Edit : " + addingBook.BookTitle;
            return View("BookDetails", addingBook);
        }
        [HttpPost]
        [Route("List/BookDetails")]
        public async Task<IActionResult> AddBookAsync(string bookId)
        {
            var currentUser = await GetCurrentUserAsync();
            SearchedBooks searchedBook = new SearchedBooks();

            searchedBook = context.SearchedBooks
          .FirstOrDefault(b => b.APIBookID == bookId);
            Book abc = new Book
            {
                BookTitle = searchedBook.BookTitle,
                AuthorFirstName = searchedBook.AuthorFirstName,
                AuthorLastName = searchedBook.AuthorLastName,
                Genre = searchedBook.Genre,
                NumberOfPages = (int)searchedBook.NumberOfPages,
                ApplicationUserId = currentUser.Id,
                Image = searchedBook.Image,
                APIBookID = searchedBook.APIBookID,
                Publisher = searchedBook.Publisher,
                PublishedDate = searchedBook.PublishedDate,
                SearchInfo = searchedBook.SearchInfo
            };

            Book extantBook = (from b in context.Books where b.BookTitle.ToLower() == abc.BookTitle.ToLower() select b).FirstOrDefault();

            if (extantBook != null)
            {

                if (context.BookUsers.ToList().Count(bu => bu.BookId == extantBook.Id
                && bu.ApplicationUserId == currentUser.Id) == 0)

                {
                    var rUserId = currentUser.Id;

                    BookUser newBookUser = new BookUser
                    {
                        BookId = extantBook.Id,
                        ApplicationUserId = rUserId,
                        ApiBookID = searchedBook.APIBookID
                    };

                    context.BookUsers.Add(newBookUser);
                    foreach (var item in context.SearchedBooks) { context.SearchedBooks.Remove(item); }
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                context.Books.Add(abc);
                await context.SaveChangesAsync();

                var bookId2 = abc.Id;
                var rUserId = currentUser.Id;

                BookUser newBookUser = new BookUser
                {
                    BookId = bookId2,
                    ApplicationUserId = rUserId,
                    ApiBookID = searchedBook.APIBookID
                };


                context.BookUsers.Add(newBookUser);
                foreach (var item in context.SearchedBooks) { context.SearchedBooks.Remove(item); }
                await context.SaveChangesAsync();

            }

            return Redirect("/Books/Index");

        }


    }
}
