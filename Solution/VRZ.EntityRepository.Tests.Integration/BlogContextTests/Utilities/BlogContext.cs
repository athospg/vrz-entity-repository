using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities
{
    public class BlogContext : DbContext
    {
        private readonly string _databaseName;

        public BlogContext()
        {
            _databaseName = "InMemoryDatabase";
        }

        public BlogContext(string databaseName)
        {
            _databaseName = databaseName;
        }

        public BlogContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);

            options
                .LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted })
                .EnableSensitiveDataLogging();

            if (!options.IsConfigured)
            {
                options.UseSqlite($"DataSource={_databaseName};mode=memory;");
            }
        }
    }
}
