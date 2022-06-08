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
using Microsoft.AspNetCore.Authorization;

namespace BookLibrary.Models
{
   //[Authorize]
   public class PostController : Controller
   {
      private BookDbContext context;
      private UserManager<ApplicationUser> _userManager;
      public PostController(BookDbContext db, UserManager<ApplicationUser> userManager) { _db = db; _userManager = userManager; }
      private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

      [HttpGet("dashboard")]
      public IActionResult Dashboard()
      //public async Task<IActionResult> DashboardAsync()
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         List<Post> allPosts = _db.Posts
             // .Include(p => p.PostBook).ThenInclude(a => a.BookTitle)
             // .Include(post => post.Comments)
             //.Include(post => post.User)

             //.Where(p => p.User.Id == _userManager.GetUserId(HttpContext.User))
             .Include(p => p.PostBook)
             .Include(p => p.User)
             .Include(post => post.Likes)
             .OrderByDescending(post => post.CreatedAt)

             .ToList();
         ViewBag.Posts = allPosts;
         return View("Dashboard", allPosts);
      }

      [HttpGet("posts/new")]
      /*public IActionResult NewPost()
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         List<Book> allBooks = _db.Books.ToList();
         ViewBag.AllBooks = allBooks;

         return View("NewPost");
      }*/
      [Authorize]
      public IActionResult NewPost(int id)
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         List<Book> allBooks = _db.Books.ToList();
         ViewBag.AllBooks = allBooks;


         Book postingBook = _db.Books
            .FirstOrDefault(b => b.Id == id);
         return View("NewPost");
      }

      [HttpPost("posts/create")]
      //public IActionResult CreatePost(Post newPost)
      // public async Task<IActionResult> CreatePost(Post newPost)
      public async Task<IActionResult> CreatePost(AddPostViewModel addPostViewModel, int bookId)
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }
         var currentUser = await GetCurrentUserAsync();

         // Book dbBook = _db.Books.FirstOrDefault(b => b.Id == bookId);
         Book postingBook = new Book();


         postingBook = _db.Books
            .Include(book => book.Posts)
            .FirstOrDefault(b => b.Id == bookId);
         //Book postingBook = _db.Books.Find(bookId);
         ViewBag.bookToPost = postingBook.BookTitle;

         //var bookId = newBook.Id;
         Post newPost = new Post
         {
            // PostTitle = newPost.PostTitle,
            ApplicationUserId = currentUser.UserName.Remove(currentUser.UserName.IndexOf("@")),
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
         .Include(p => p.Likes)
         .Include(p => p.PostBook)
         //.Where(p => p.BookId == id)
         // .Include(p => p.User)
         .OrderByDescending(post => post.CreatedAt)
         .ToList();

         List<Post> postsToShow = new List<Post>();
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


      [Authorize]
      [HttpPost("posts/{postId}/like")]
      public async Task<IActionResult> LikeAsync(int postId)
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         var currentUser = await GetCurrentUserAsync();

         /*ApplicationUser applicationUser = _userManager
            .FindByNameAsync(User.Identity.Name).Result;*/

         ApplicationUser dbUser = _db.Users
            .Include(u => u.Likes)
            .FirstOrDefault(u => u.Id == currentUser.Id);
         Post post = _db.Posts
            .Include(p => p.User)
            .Include(post => post.Likes)
            .FirstOrDefault(p => p.PostId == postId);
         Like like = _db.Likes.FirstOrDefault(l => l.PostId == postId && l.LikeUser.Id == currentUser.Id);


         // User hasn't liked this post yet
         if (like == null)
         {
            Like newLike = new Like
            {
               ApplicationUserId = currentUser.Id,
               PostId = postId,
               UserId = dbUser.Id,
               LikeUser = dbUser,
               LikePost = _db.Posts.FirstOrDefault(p => p.PostId == postId)
            };

            post.Likes.Add(newLike);
            currentUser.Likes.Add(newLike);

            _db.Likes.Add(newLike);
            // _db.SaveChanges();
         }
         else
         {
            post.Likes.Remove(like);
            currentUser.Likes.Remove(like);
            _db.Likes.Remove(like);
            //_db.SaveChanges();
         }
         ViewBag.CurrentUser = currentUser.Id;


         _db.SaveChanges();
         return RedirectToAction("Dashboard");
      }

      /*[HttpPost("posts/{postId}/update")]*/
      /*public IActionResult UpdatePost(int postId, Post updatedPost)
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         Post post = _db.Posts.FirstOrDefault(p => p.PostId == postId);
         {
           // var currentUser = await GetCurrentUserAsync();
            if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

            if (!ModelState.IsValid)
            {
               List<Book> allBooks = _db.Books.ToList();
               ViewBag.AllBooks = allBooks;
               return View("NewPost");
            }
         }
      }*/
      // ApplicationUser dbUser = await GetCurrentUserAsync();

      /*newPost.User = currentUser;
      newPost.PostUser = dbUser;*/
      /*ApplicationUser dbUser = _db.Users.FirstOrDefault(user => user.UserName == currentUser.UserName)
             .Include(bookuser => user.Posts)
             .FirstOrDefault(user => user.UserId == _UserId);*/
      /*BookUser dbUser = _db.BookUsers
             .Include(bookuser => bookuser.Posts)
             .FirstOrDefault(user => user.ApplicationUserId == currentUser.Id);*/


      /* newPost.ApplicationUserId = currentUser.Id;
       newPost.PostUser = dbUser;*/
      /* Book dbBook = _db.Books
           //.Include(album => album.AlbumRatings)
              .Include(book => book.Posts)
              .FirstOrDefault(book => book.Id == newPost.BookId);*/
      /* Book dbBook = (from b in context.Books where b.ApplicationUserId == newPost.ApplicationUserId select b).FirstOrDefault();

       if (dbBook == null) { return RedirectToAction("Dashboard"); }*/

      /*List<Post> omg2 = (from b in context.Books
                        join bu in context.BookUsers on b.Id equals bu.BookId
                        join a in context.ApplicationUsers on bu.ApplicationUserId equals a.Id
                        where a.Id == currentUser.Id
                        select new Post
                        {
                           ApplicationUserId = currentUser.Id,
                           //PostUser = dbUser,
                           BookId = newPost.BookId,
                           PostBook = dbBook,
                           //Title = newPost.Title,
                           Content = newPost.Content,
                           CreatedAt = DateTime.Now,
                           UpdatedAt = DateTime.Now
                        }).ToList<Post>();*/

      //Book dbBook = (from b in context.Books where b.ApplicationUserId == post.ApplicationUserId select b).FirstOrDefault();

      /* Post post = new Post
       {
          ApplicationUserId = currentUser.Id,
          //PostUser = dbUser,
          //BookId = post.BookId,
          PostBook = dbBook,
          //Title = newPost.Title,
          Content = AddPostViewModel.Content,
          CreatedAt = DateTime.Now,
          UpdatedAt = DateTime.Now
       };
       Book dbBook = (from b in context.Books where b.ApplicationUserId == post.ApplicationUserId select b).FirstOrDefault();

       if (dbBook == null) { return RedirectToAction("Dashboard"); }

       *//*AlbumRating newAlbumRating = new AlbumRating()
       {
           Rating = newPost.AlbumRatingNumber,
           AlbumId = newPost.AlbumId,
           RatingAlbum = dbAlbum,
           PostId = newPost.PostId,
           RatingPost = newPost
       };*//*

       //newPost.PostRating = newAlbumRating;
       //newPost.RatingId = newAlbumRating.AlbumRatingId;

       // dbAlbum.AlbumRatings.Add(newAlbumRating);
       dbBook.Posts.Add(newPost);
       dbUser.Posts.Add(newPost);
       //BookUser.Posts.Add(newPost);

       _db.Posts.Add(newPost);
       //_db.AlbumRatings.Add(newAlbumRating);
       _db.SaveChanges();
       return RedirectToAction("Dashboard");
    }*/
      /*[HttpPost("posts/{postId}/like")]
      public async Task<IActionResult> LikeAsync(int postId)
      {
         if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }

         Post post = _db.Posts.FirstOrDefault(p => p.PostId == postId);
         if (post == null) { return RedirectToAction("Index", "Home"); }

         // ApplicationUser currentUser = _db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
         ApplicationUser currentUser = await GetCurrentUserAsync();
         if (currentUser == null) { return RedirectToAction("Index", "Home"); }

         Like newLike = new Like()
         {
            PostId = postId,
            UserId = currentUser.FirstName
         };
         _db.Likes.Add(newLike);
         _db.SaveChanges();
         return RedirectToAction("ShowPost", new { postId = postId });
      }*/
      /*[HttpPost("posts/{postId}/like")]
      public IActionResult Like(int postId)
      {
         if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }

         User dbUser = _db.Users
             .Include(user => user.Likes)
             .FirstOrDefault(user => user.UserId == _UserId);
         Post dbPost = _db.Posts
             .Include(post => post.Likes)
             .FirstOrDefault(post => post.PostId == postId);
         Like dbLike = _db.Likes
             .FirstOrDefault(like => like.PostId == postId && like.UserId == _UserId);

         // User hasn't liked this post yet
         if (dbLike == null)
         {
            Like newLike = new Like()
            {
               LikeUser = dbUser,
               LikePost = dbPost,
               UserId = (int)_UserId,
               PostId = postId
            };

            dbPost.Likes.Add(newLike);
            dbUser.Likes.Add(newLike);

            _db.Likes.Add(newLike);
         }

         // User has already liked this post and is unliking it
         else
         {
            dbPost.Likes.Remove(dbLike);
            dbUser.Likes.Remove(dbLike);
            _db.Likes.Remove(dbLike);
         }

         _db.SaveChanges();

         return RedirectToAction("Dashboard");
      }*/

      /*[HttpPost("likes/{postId}/comment")]
      public IActionResult Comment(int postId, Comment newComment)
      {
          if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }

          User dbUser = _db.Users
              .FirstOrDefault(user => user.UserId == _UserId);
          Post dbPost = _db.Posts
              .FirstOrDefault(post => post.PostId == postId);

          newComment.PostId = postId;
          newComment.CommentPost = dbPost;
          newComment.UserId = (int)_UserId;
          newComment.CommentUser = dbUser;

          if (dbPost.Comments == null) { dbPost.Comments = new List<Comment>(); }
          if (dbUser.Comments == null) { dbUser.Comments = new List<Comment>(); }
          dbUser.Comments.Add(newComment);
          dbPost.Comments.Add(newComment);

          _db.Comments.Add(newComment);
          _db.SaveChanges();

          return RedirectToAction("ViewComments", new { postId = postId });
      }*/

      /* [HttpGet("posts/{postId}")]
       public IActionResult ViewComments(int postId)
       {
           if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }

           Post dbPost = _db.Posts
               .Include(post => post.Likes)
               .Include(post => post.Comments)
                   .ThenInclude(comment => comment.CommentUser)
               .Include(post => post.PostUser)
               .Include(post => post.PostAlbum)
                   .ThenInclude(album => album.AlbumArtist)
               .FirstOrDefault(post => post.PostId == postId);

           return View("ViewComments", dbPost);
       }*/
      //-------------------------------BELOW THIS LINE WAS ALREADY COMMENTED OUT-------------------------------------

      /* [HttpGet("posts/{postId}")]
      public IActionResult Details(int postId)
      {
          if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }
      }

      // If we want to be able to create a new post only directly from the
      // dashboard like Facebook, this route is unnecessary. Having a
      // separate page is probably simpler though.

      [HttpGet("posts/{postId}/edit")]
      public IActionResult Edit(int postId)
      {
          if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }
      }

      [HttpPost("posts/{postId}/update")]
      public IActionResult Update(int postId, Post updatedPost)
      {
          if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }
          if (!ModelState.IsValid) { return View("Edit"); }
      }

      [HttpGet("posts/{postId}/delete")]
      public IActionResult Delete(int postId)
      {
          if (!_IsLoggedIn) { return RedirectToAction("Index", "Home"); }
      } */

      // Database
      private BookDbContext _db;


      // Utility attributes
      //private int? _UserId { get => HttpContext.Session.GetInt32("UserId"); }
      //private bool _IsLoggedIn { get => _UserId != null; }
   }
}