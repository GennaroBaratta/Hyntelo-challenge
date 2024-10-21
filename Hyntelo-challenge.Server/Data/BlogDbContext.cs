﻿using Hyntelo_challenge.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Hyntelo_challenge.Server.DB
{
    public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId);

            // Seed some initial data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "admin123", Name = "Administrator" },
                new User { Id = 2, Username = "user", Password = "user123", Name = "Regular User" }
            );
        }
    }
}