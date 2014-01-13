using NewsFeed.Data.Migrations;
using NewsFeed.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Collections.Generic;

namespace NewsFeed.Data
{
    public class NewsFeedContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Friend> Friends { get; set; }

        public NewsFeedContext()
            : base("NewsFeedDB")
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<NewsFeedContext, Configuration>());
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }
}
