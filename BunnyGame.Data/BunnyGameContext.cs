using BunnyGame.Models;
using System;
using System.Data.Entity;

namespace BunnyGame.Data
{
    public class BunnyGameContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }

        public BunnyGameContext()
            : base("BunnyGameDB")
        { }
    }
}
