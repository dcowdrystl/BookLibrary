using BookLibrary.Data;
using BookLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using BookLibrary.ViewModels;
using System.Security.Policy;
using System.Net;
//AIzaSyACdHSQbarZ_V5SzuDEg8UQUQQX_tKfpvA
namespace BookLibrary.Controllers
{
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
      /*public IActionResult Index()
      {
          List<Book> books = context.Books.ToList();

          return View(books);
      }*/
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


      public async Task<IActionResult> ShowGenre(string searchTerm, string genre)
      {
         Book book1 = new Book();
         
         List<Book> books = context.Books
         .Where(b => b.Genre == genre)
         .ToList();
         ViewBag.allBooks = books.Count();

         /* List<Book> booksToShow = new List<Book>();
          foreach (Book book in books)
          {
             if (book.Genre == genre)
             {
                booksToShow.Add(book);
             }
             booksToShow.ToList();
          }*/

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

        [HttpPost]
        public async Task<IActionResult> GetBooksAsync(string searchTerm)
        {
            if (!ModelState.IsValid) { return View(); }
            if (ModelState.IsValid)
            {
                ViewBag.testing = new List<SearchedBooks>();
                var currentUser = await GetCurrentUserAsync();
                var queryList = service.Volumes.List(searchTerm);
                queryList.MaxResults = 5;

                // if queryList contains an object without an ImageLink, remove it from the List

                //queryList.Filter = (VolumesResource.ListRequest.FilterEnum?)VolumesResource.ListRequest.PrintTypeEnum.BOOKS;

                //queryList.Filter = VolumesResource.ListRequest.FilterEnum.Partial;
                var result = queryList.Execute();
                ViewBag.result = result;
            
                foreach (var resultItem in result.Items)
                {
                    /*if (resultItem.VolumeInfo.ImageLinks != null)
                    {
                       continue;
                    }
                    else
                    {
                    }*/

                    
                    if (result != null) { 
                       
                       /* if(resultItem.VolumeInfo.ImageLinks.Thumbnail == null)
                        {
                            result.Items.Remove(resultItem);
                        }*/
                 //  if(resultItem.VolumeInfo.ImageLinks.Thumbnail.Any())
                    // if (resultItem.VolumeInfo.ImageLinks.GetType() != null)
                    

                        var booksApi = result.Items  
                            .Where(b => b.VolumeInfo.Categories != null)
                            .Where(b => b.VolumeInfo.ImageLinks.Thumbnail != null)
                            .Where(b => b.VolumeInfo.Title != null)
                            .Where(b => b.VolumeInfo.Authors != null)
                            .Where(b => b.VolumeInfo.PageCount != null)
                            .Select(b => new SearchedBooks
                        {
                            // Id = context.Books.Count() + 1,
                            BookTitle = b.VolumeInfo.Title,
                           // AuthorFirstName = b.VolumeInfo.Authors[0],
                            AuthorLastName = b.VolumeInfo.Authors.FirstOrDefault(),
                            Genre = b.VolumeInfo.Categories[0],
                            NumberOfPages = (int)b.VolumeInfo.PageCount,
                            ApplicationUserId = currentUser.Id,
                            Image = b.VolumeInfo.ImageLinks.Thumbnail,
                            APIBookID = b.Id

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
                                    APIBookID = testBook.APIBookID
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
                                    APIBookID = testBook.APIBookID
                                };
                               // ViewBag.testing.Add(hello);
                              //  ViewBag.testing.Add(testing);
                              // foreach (var tb in context.Books)
                              //  {
                                  //  if (tb.ApplicationUserId != hello.ApplicationUserId)
                                  //  {

                                    
                                    context.SearchedBooks.Add(hello);
                                        //context.SaveChanges();
                                        await context.SaveChangesAsync();
                                        
                                       /* var bookId = hello.Id;
                                        var rUserId = currentUser.Id;

                                        BookUser newBookUser = new BookUser
                                        {
                                            BookId = bookId,
                                            ApplicationUserId = rUserId
                                        };


                                        context.BookUsers.Add(newBookUser);
                                        await context.SaveChangesAsync();*/
                                   // }
                              //  }
                            }
                        }
                        // testing.ToList();
                        ViewBag.testingBooks = testing;
                         return View("SearchedBooks", testing);
                        //return Redirect("/List");

                    }
                    
                    else
                    {
                        return null;
                    }

                }


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
                if(book.APIBookID == bookId)
                {
                    addingBook = book;
                  /*  context.SearchedBooks.Add(addingBook);
                    context.SaveChanges();*/
                }
            }
            /*Book addingBook = new Book();
            addingBook = context.Books
                   .FirstOrDefault(b => b.APIBookID == bookId);*/
           // addingBook.Id = context.Books.Count() + 1;
            ViewBag.bookToAdd = addingBook;
            ViewBag.title = "Edit : " + addingBook.BookTitle;
            return View("BookDetails", addingBook);
        }
        [HttpPost]
        [Route("List/BookDetails")]
        //public IActionResult CreatePost(Post newPost)
        // public async Task<IActionResult> CreatePost(Post newPost)
        public async Task <IActionResult> AddBookAsync(string bookId)
      {
            // Book book = new Book();
            //foreach (var book2 in ViewBag.testingBooks)
            // {
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
                APIBookID = searchedBook.APIBookID
            };
            /*if (abc.APIBookID != null)
                {
                //Book hello = ViewBag.testingBooks.Find(apibookID);
                *//* context.Books.Add(searchedBook);
             context.SaveChanges();*//*
                // await context.SaveChangesAsync();
                context.Books.Add(abc);
                await context.SaveChangesAsync();
               
            }*/

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
                        ApplicationUserId = rUserId
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
                    ApplicationUserId = rUserId
                };


                context.BookUsers.Add(newBookUser);
                foreach (var item in context.SearchedBooks) { context.SearchedBooks.Remove(item); }
                await context.SaveChangesAsync();

            }
            //context.SaveChanges();
            return Redirect("Index");
           //  context.SaveChanges();
           // return View("SearchedBooks", ViewBag.testingBooks);
        }
  
    
    }
}
