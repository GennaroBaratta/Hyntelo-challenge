﻿using Hyntelo_challenge.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Hyntelo_challenge.Server.DB
{
    public class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
