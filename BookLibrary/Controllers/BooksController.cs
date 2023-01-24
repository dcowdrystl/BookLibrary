using BookLibrary.Data;
using BookLibrary.Models;
using BookLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookLibrary.Controllers
{
   [Authorize]
   public class BooksController : Controller
   {
      private BookDbContext context;
      private UserManager<ApplicationUser> _userManager;

      public BooksController(BookDbContext dbContext, UserManager<ApplicationUser> userManager)
      {
         context = dbContext;
         _userManager = userManager;
      }

      private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


      //[AllowAnonymous]
      public async Task<IActionResult> Index()
      {
         if (!ModelState.IsValid) { return View(); }

         if (ModelState.IsValid)
         {
                var currentUser = await GetCurrentUserAsync();

                List<Book> books = context.Books
                    .OrderByDescending(b => b.CreatedAt)
                    .Include(b => b.BookUsers)
                    .Include(a => a.User)
                    //.ThenInclude(bu => bu.ApplicationUser)
                    .Where(b => b.BookUsers.Any(bu => bu.ApplicationUserId == currentUser.Id))                 
                    .ToList();
                ViewBag.MyBooks = books.Count();
                ViewBag.PersonalBooks = books;

                /* List<Book> omg = (from b in context.Books
                                   join bu in context.BookUsers on b.Id equals bu.BookId
                                   join a in context.ApplicationUsers on bu.ApplicationUserId equals a.Id
                                   where a.Id == currentUser.Id
                                   orderby b.AuthorLastName ascending
                                   select new Book

                                   {
                                      Id = b.Id,
                                      BookTitle = b.BookTitle,
                                      AuthorFirstName = b.AuthorFirstName,
                                      AuthorLastName = b.AuthorLastName,
                                      Genre = b.Genre,
                                      NumberOfPages = b.NumberOfPages,
                                   }).ToList<Book>();*/
                /* var currentUser = await GetCurrentUserAsync();
                 List<Book> books = context.Books.Where(b => b.ApplicationUserId == currentUser.Id)

               .ToList();*/

                return View(books);
         }

         return View();
      }

      public IActionResult Add()
      {


         AddBookViewModel addBookViewModel = new AddBookViewModel();
         return View(addBookViewModel);
      }

      [HttpPost]
      public async Task<IActionResult> Add(AddBookViewModel addBookViewModel)
      {
         var currentUser = await GetCurrentUserAsync();
         if (!ModelState.IsValid)
         {

            return View("Add", addBookViewModel);

         }
         else if (ModelState.IsValid)
         {


            Book newBook = new Book
            {
               BookTitle = addBookViewModel.BookTitle,
               AuthorFirstName = addBookViewModel.AuthorFirstName,
               AuthorLastName = addBookViewModel.AuthorLastName,
               Genre = addBookViewModel.Genre,
               NumberOfPages = addBookViewModel.NumberOfPages,
               ApplicationUserId = currentUser.Id,
               Image = addBookViewModel.Image

            };

            Book extantBook = (from b in context.Books where b.BookTitle.ToLower() == newBook.BookTitle.ToLower() select b).FirstOrDefault();

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
                  await context.SaveChangesAsync();
               }
            }
            else
            {
               context.Books.Add(newBook);
               await context.SaveChangesAsync();

               var bookId = newBook.Id;
               var rUserId = currentUser.Id;

               BookUser newBookUser = new BookUser
               {
                  BookId = bookId,
                  ApplicationUserId = rUserId
               };


               context.BookUsers.Add(newBookUser);
               await context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
         }
         return View(addBookViewModel);
      }
      //[HttpGet]
      //public IActionResult Delete()
      //{
      //    /*ViewBag.books = BookData.GetAll();*/
      //    ViewBag.books = context.Books.ToList();
      //    return RedirectToAction(nameof(Index));
      //}

      [HttpGet]
      public async Task<IActionResult> Delete(int bookId)
      {
         var currentUser = await GetCurrentUserAsync();


         BookUser extantUser = (from bu in context.BookUsers
                                where bu.BookId == bookId && bu.ApplicationUserId == currentUser.Id
                                select bu).FirstOrDefault();
           
         if (extantUser != null)
         {
            context.BookUsers.Remove(extantUser);
            context.SaveChanges();

            extantUser = (from bu in context.BookUsers
                          where bu.BookId == bookId
                          select bu).FirstOrDefault();

            if (extantUser == null)
            {
               Book extantBook = context.Books.Find(bookId);
               context.Books.Remove(extantBook);
               context.SaveChanges();
            }

         }

         context.SaveChanges();
         return RedirectToAction(nameof(Index));
      }

      [HttpGet]
      [Route("Books/Edit/{bookId}")]
      public IActionResult Edit(int bookId)
      {

         Book editingBook = context.Books.Find(bookId);

         ViewBag.bookToEdit = editingBook;
         ViewBag.title = "Edit : " + editingBook.BookTitle;
         return View();
      }

      [HttpPost]
      [Route("Books/Edit")]
      public IActionResult SubmitEditBookForm(int bookId, string booktitle, string authorfirstname, string authorlastname, string genre, int numberofpages, string image)
      {

         Book editingBook = context.Books.Find(bookId);
         editingBook.BookTitle = booktitle;
         editingBook.AuthorFirstName = authorfirstname;
         editingBook.AuthorLastName = authorlastname;
         editingBook.Genre = genre;
         editingBook.NumberOfPages = numberofpages;
         editingBook.Image = image;

         context.SaveChanges();
         return Redirect("/Books");
      }

      public async Task<IActionResult> TheirBooksAsync(string userName, string userId)
      {
         ViewBag.bookTitles = new List<string>();

         var findUserBooks = (from b in context.Books
                              join bu in context.BookUsers on b.Id equals bu.BookId
                              join a in context.ApplicationUsers on bu.ApplicationUserId equals a.Id
                              where a.Id == userId
                              select new Book {
                                 Id = b.Id,
                                 BookTitle = b.BookTitle,
                                 AuthorFirstName = b.AuthorFirstName,
                                 AuthorLastName = b.AuthorLastName,
                                 Genre = b.Genre,
                                 NumberOfPages = b.NumberOfPages,
                                 Image = b.Image
                              }).ToList<Book>();
         ViewBag.User = userName
             .Remove(userName.IndexOf("@"));
         ViewBag.Uname = userName;
         ViewBag.Uid = userId;
         if (User.Identity.IsAuthenticated)
         {
            var currentUser = await GetCurrentUserAsync();

            var titles = (from b in context.Books
                          join bu in context.BookUsers on b.Id equals bu.BookId
                          join au in context.ApplicationUsers on bu.ApplicationUserId equals au.Id
                          where au.Id == currentUser.Id
                          select new Book
                          {
                             Id = b.Id,
                             BookTitle = b.BookTitle,
                             AuthorFirstName = b.AuthorFirstName,
                             AuthorLastName = b.AuthorLastName,
                             Genre = b.Genre,
                             NumberOfPages = b.NumberOfPages,
                             ApplicationUserId = "",
                             Image = b.Image
                          }).ToList();

            foreach (var book in titles)
            {

               ViewBag.bookTitles.Add(book.BookTitle);
            }
         }
         return View(findUserBooks);
      }


      public async Task<IActionResult> AddOtherBooks(int bookId, string url)
      {
         var currentUser = await GetCurrentUserAsync();


         if (context.BookUsers.ToList().Count(bu => bu.BookId == bookId
                 && bu.ApplicationUserId == currentUser.Id) == 0)

         {




            var rUserId = currentUser.Id;


            BookUser newBookUser = new BookUser
            {
               BookId = bookId,
               ApplicationUserId = rUserId
            };

            context.BookUsers.Add(newBookUser);
            await context.SaveChangesAsync();
            return Redirect(url);


         }
         context.SaveChanges();
         return Redirect(url);
         //return RedirectToAction(nameof(Index));
      }

      public IActionResult Detail(int id)
      {
         Book theBook = context.Books
            .Include(e => e.User)
            .Single(e => e.Id == id);


         var findLikedBooks = (from u in context.Profiles
                               join bu in context.BookUsers on u.ApplicationUserId equals bu.ApplicationUserId
                               join a in context.ApplicationUsers on bu.ApplicationUserId equals a.Id
                               where bu.BookId == id
                               select new UserProfile
                               {
                                  Id = u.Id,
                                  UserName = u.UserName,
                                  ApplicationUserId = u.ApplicationUserId

                               }).ToList();

         var self = findLikedBooks.Find(x => x.UserName == User.Identity.Name);
         findLikedBooks.Remove(self);

         BookDetailViewModel viewModel = new BookDetailViewModel(theBook, findLikedBooks);
         return View(viewModel);
      }

      [HttpPost]
      public IActionResult AuthorsLastNameOrdered()
      {
         if (!ModelState.IsValid) { return View(); }

         if (ModelState.IsValid)
         {
           

            // This ONE line of code below took about 9 hours to create...


            List<Book> omg = (from b in context.Books
                              join bu in context.BookUsers on b.Id equals bu.BookId
                              join a in context.ApplicationUsers on bu.ApplicationUserId equals a.Id                         
                              orderby b.AuthorLastName ascending
                              select new Book

                              {
                                 Id = b.Id,
                                 BookTitle = b.BookTitle,
                                 AuthorFirstName = b.AuthorFirstName,
                                 AuthorLastName = b.AuthorLastName,
                                 Genre = b.Genre,
                                 NumberOfPages = b.NumberOfPages,
                              }).ToList<Book>();




            return View(omg);
         }

         return View();


      }

      public async Task<IActionResult> ShowGenre(string genre)
      {
         var currentUser = await GetCurrentUserAsync();

         List<Book> books = context.Books
                .OrderByDescending(b => b.CreatedAt)
             .Include(b => b.BookUsers)
             .Include(a => a.User)
             //.ThenInclude(bu => bu.ApplicationUser)
             .Where(b => b.BookUsers.Any(bu => bu.ApplicationUserId == currentUser.Id))
             .Where(b => b.Genre == genre)
             .ToList();
         ViewBag.MyBooks = books.Count();
         return View("Index", books);
      }

        [HttpGet]
        public async Task<IActionResult> Favorite(string bookId)
        {
            var currentUser = await GetCurrentUserAsync();


            BookUser extantUser = (from bu in context.BookUsers
                                   where bu.ApiBookID == bookId && bu.ApplicationUserId == currentUser.Id
                                   select bu).FirstOrDefault();

            if (extantUser != null)
            {
               extantUser.isFavorite = true;
                context.SaveChanges();

            }

            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ShowFavorites()
        {
            var currentUser = await GetCurrentUserAsync();

            List<Book> fbooks = context.Books
                   .OrderByDescending(b => b.CreatedAt)
                   .Include(b => b.BookUsers)
                   .Include(a => a.User)
                   //.ThenInclude(bu => bu.ApplicationUser)
                   .Where(b => b.BookUsers.Any(bu => bu.ApplicationUserId == currentUser.Id && bu.isFavorite == true))
                   .ToList();





            return View("Index", fbooks);
        }

    }
}
