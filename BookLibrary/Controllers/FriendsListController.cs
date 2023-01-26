using BookLibrary.Data;
using BookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BookLibrary.Controllers
{
    [Authorize]
    public class FriendsListController : Controller
    {
        private readonly BookDbContext _context;

        public FriendsListController(BookDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var people = _context.Users.ToList();
            var friends = _context.Friends
                .Include(x => x.User)
                .Include(x => x.Friend)
                .ToList();
            //Remove friends I already have from list
            ViewBag.friendNames = new List<string>();
            foreach (var relationship in friends)
            {
                if (User.Identity.Name == relationship.User.UserName)
                {
                    ViewBag.friendNames.Add(relationship.Friend.UserName);
                }
            }

            //Remove myself from friends options
            var self = people.Find(x => x.UserName == User.Identity.Name);
            people.Remove(self);
            return View(people);
        }
        public UserProfile GetUser()
        {
            var user = _context.Users.First(p => p.UserName == User.Identity.Name);
            var doesUserHaveProfile = _context.Profiles.FirstOrDefault(u => u.UserName == user.UserName);

            if (doesUserHaveProfile == null)
            {
                UserProfile profile = new UserProfile
                {
                    UserName = user.UserName,
                    ApplicationUserId = user.Id
                };
                _context.Profiles.Add(profile);
                _context.SaveChanges();
                return profile;
            }
            else
            {
                return doesUserHaveProfile;
            }

        }


        public UserProfile GetFriend(string id)
        {
            var user = _context.Users.First(p => p.Id == id);
            var doesFriendHaveProfile = _context.Profiles.FirstOrDefault(u => u.UserName == user.UserName);

            if (doesFriendHaveProfile == default)
            {
                UserProfile friendProfile = new UserProfile
                {
                    UserName = user.UserName,
                    ApplicationUserId = user.Id,
                };
                _context.Profiles.Add(friendProfile);
                _context.SaveChanges();
                return friendProfile;
            }
            return doesFriendHaveProfile;
        }

        public int GetRelationId(UserProfile user, UserProfile friend)
        {
            var areFriends = _context.Friends.FirstOrDefault(x => x.UserId == user.Id && x.FriendId == friend.Id);
            return areFriends.RelationId;
        }

        public IActionResult AddFriend(string id)
        {
            var user = GetUser();
            var newFriend = GetFriend(id);

            Friends besties = new Friends
            {
                UserId = user.Id,
                FriendId = newFriend.Id
            };
            _context.Friends.Add(besties);
            _context.SaveChanges();
            return RedirectToAction("MyFriends");
        }

        public IActionResult MyFriends()
        {
            var user = GetUser();
            var myFriends = _context.Friends.Where(x => x.UserId == user.Id)
                .Include(x => x.User)
                .Include(x => x.Friend)
                .ToList();

            return View(myFriends);
        }

        public IActionResult RemoveFriend(int id)
        {
            var user = GetUser();
            var friend = _context.Profiles.First(x => x.Id == id);
            var toRemove = _context.Friends.First(x => x.Friend.UserName == friend.UserName && x.User.UserName == user.UserName
            || x.Friend.UserName == user.UserName && x.User.UserName == friend.UserName);


            _context.Friends.Remove(toRemove);
            _context.SaveChanges();

            return RedirectToAction("MyFriends");
        }
    }
}
