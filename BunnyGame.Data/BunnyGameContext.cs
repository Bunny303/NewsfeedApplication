using BunnyGame.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BunnyGame.Data
{
    public class BunnyGameContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Friend> Friends { get; set; }

        public BunnyGameContext()
            : base("NewsFeedDB")
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
