﻿using BookLibrary.Data;
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
         List<Book> books = context.Books
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
                          select new Book
                          {
                             Id = b.Id,
                             BookTitle = b.BookTitle,
                             AuthorFirstName = b.AuthorFirstName,
                             AuthorLastName = b.AuthorLastName,
                             Genre = b.Genre,
                             NumberOfPages = b.NumberOfPages,
                             ApplicationUserId = ""
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
                          select new Book
                          {
                             Id = b.Id,
                             BookTitle = b.BookTitle,
                             AuthorFirstName = b.AuthorFirstName,
                             AuthorLastName = b.AuthorLastName,
                             Genre = b.Genre,
                             NumberOfPages = b.NumberOfPages,
                             ApplicationUserId = ""
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


                    if (result != null)
                    // if (resultItem.VolumeInfo.ImageLinks.GetType() != null)
                    {

                  var booksApi = result.Items.Select(b => new Book
                  {

                     BookTitle = b.VolumeInfo.Title,
                     //AuthorFirstName = b.VolumeInfo.Authors[0],
                     AuthorLastName = b.VolumeInfo.Authors.FirstOrDefault(),
                     //Genre = b.VolumeInfo.Categories[0],
                     NumberOfPages = (int)b.VolumeInfo.PageCount,

                     Image = b.VolumeInfo.ImageLinks.Thumbnail,
                     APIBookID = b.Id

                  })
                           .ToList();



                        ViewBag.booksApi = booksApi;
                        List<Book> testing = new List<Book>();
                        foreach (var testBook in booksApi)
                        {

                     Book hello = new Book
                     {
                        Id = testBook.Id,
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
                            ViewBag.testing = testing;
                            context.Books.Add(hello);
                            context.SaveChanges();
                        }
                        // testing.ToList();

                         return View("Index", testing);
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
        [HttpPost]
      //public IActionResult CreatePost(Post newPost)
      // public async Task<IActionResult> CreatePost(Post newPost)
      public async Task<IActionResult> AddBook(string apibookID)
      {
            Book hello = context.Books.Find(apibookID);
         context.Books.Add(hello);
            context.SaveChanges();
            return Redirect("Index");
        }
   }
}
