using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using BookLibrary.Models;
using Microsoft.EntityFrameworkCore;
using BookLibrary.Data;
using Microsoft.AspNetCore.Identity;


using Microsoft.EntityFrameworkCore.Internal;


using System.Threading.Tasks;
using BookLibrary.ViewModels;

namespace BookLibrary.Models
{
   public class PostController : Controller
   {
      private BookDbContext context;
      private UserManager<ApplicationUser> _userManager;
      public PostController(BookDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }
      private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

      [HttpGet("dashboard")]
      public IActionResult Dashboard()
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         List<Post> allPosts = _db.Posts
             .Include(p => p.PostBook)
             .Include(p => p.User)
             .OrderByDescending(post => post.CreatedAt)
             
             .ToList();
      
         return View("Dashboard", allPosts);
      }

      [HttpGet("posts/new")]
      public IActionResult NewPost()
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         List<Book> allBooks = _db.Books.ToList();
         ViewBag.AllBooks = allBooks;

         return View("NewPost");
      }
     /* public IActionResult NewPost(int id)
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         List<Book> allBooks = _db.Books.ToList();
         ViewBag.AllBooks = allBooks;
         
         
         Book postingBook = _db.Books
            .FirstOrDefault(b => b.Id == id);
         return View("NewPost");
      }*/

      [HttpPost("posts/create")]
      public async Task<IActionResult> CreatePost(AddPostViewModel addPostViewModel, int bookId)
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }
         var currentUser = await GetCurrentUserAsync();

         Book postingBook = new Book();       
         
         postingBook = _db.Books.FirstOrDefault(b => b.Id == bookId);
         ViewBag.bookToPost = postingBook.BookTitle;

         Post newPost = new Post
         {
            ApplicationUserId = currentUser.FirstName,
            Content = addPostViewModel.Content,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            //UserId = _db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id,
            UserId = currentUser.FirstName,
            BookId = bookId,
            User = _db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name),
            // PostBook = _db.Books.FirstOrDefault(b => b.Id == bookId)
            // PostBook = postingBook
            PostBook = _db.Books.FirstOrDefault(b => b.Id == bookId)
            //PostBook = _db.Books.Find(bookId)
         };
         

         
         _db.Posts.Add(newPost);
  
         _db.Books.Find(bookId).Posts.Add(newPost);
         
      
         //_db.Users.Find(currentUser.Id).Posts.Add(newPost);
         currentUser.Posts.Add(newPost);
         _db.SaveChanges();
         return RedirectToAction("Dashboard");
      }

      [HttpGet]
      [Route("Posts/ShowPost")]
      public IActionResult ShowPost(int id)
     /* {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         Post post = _db.Posts.Include(p => p.PostBook).Include(p => p.User).FirstOrDefault(p => p.Id == id);
         ViewBag.Post = post;
         return View("ShowPost");
      }*/
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }
         
         Book postingBook = new Book();
         postingBook = _db.Books
            .FirstOrDefault(b => b.Id == id);

         List<Post> bookPosts = _db.Posts        
         .Include(p => p.PostBook)
        // .Include(p => p.User)
         .OrderByDescending(post => post.CreatedAt)
         .ToList();

         List< Post > postsToShow = new List<Post>();
         foreach (Post post in bookPosts)
         {
            //only add posts to postsToShow that are associated with PostBook

            if (id == post.PostBook.Id)
            {
               postsToShow.Add(post);
            }

            postsToShow.ToList();
         }

         return View("Dashboard", postsToShow);
      }


      /*[HttpGet("posts/{postId}/edit")]
      public IActionResult EditPost(int postId)
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         Post post = _db.Posts
             .Include(p => p.PostBook).ThenInclude(a => a.BookTitle)
             .Include(post => post.Comments)
             .FirstOrDefault(p => p.PostId == postId);
         ViewBag.Post = post;
         return View("EditPost");
      }*/

      
      private BookDbContext _db;

    }
}