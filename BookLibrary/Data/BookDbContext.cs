﻿using BookLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookLibrary.Data
{
    public class BookDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<BookUser> BookUsers { get; set; }

        public DbSet<Friends> Friends { get; set; }

        public DbSet<UserProfile> Profiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<SearchedBooks> SearchedBooks { get; set; }
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        { 
        }
      

      //protected override void OnModelCreating(ModelBuilder modelBuilder)
      //{

      //    base.OnModelCreating(modelBuilder);
      //}

      protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
         {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
         }

         modelBuilder.Entity<BookUser>()
                .HasKey(bu => new { bu.BookId, bu.ApplicationUserId });

            /*modelBuilder.Entity<SearchedBooks>(eb =>
            {
                eb.HasNoKey();
            });
                */

            base.OnModelCreating(modelBuilder);

            /*modelBuilder.Entity<Book>(b =>
            {
                b.Property(u => u.Id).HasDefaultValueSql("newsequentialid()");
            });*/

        }
    }
}