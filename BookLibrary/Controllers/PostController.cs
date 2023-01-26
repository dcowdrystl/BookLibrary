using BookLibrary.Data;
using BookLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


        // Database
        private BookDbContext _db;

    }
}