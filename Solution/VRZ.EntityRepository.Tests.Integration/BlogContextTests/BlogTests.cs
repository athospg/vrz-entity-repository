using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRZ.EntityRepository.SDK.EntityRepository;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models;
using Xunit;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests
{
    public class GetBlogTests
    {
        private readonly BlogContext _context;
        private readonly IEntityRepository<long, Blog> _blogsRepository;

        public GetBlogTests()
        {
            _context = new BlogContext();

            _context.Database.OpenConnection();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            Utilities.Utilities.InitializeDbForTests(_context);

            _blogsRepository = new EntityRepository<Blog>(_context);
        }

        ~GetBlogTests()
        {
            _context.Dispose();
        }


        [Fact]
        public async Task Get_Blogs_CountAll()
        {
            // Arrange

            // Act
            var blogsCount = await _blogsRepository.CountAll();

            // Assert
            Assert.Equal(Utilities.Utilities.BlogsCount, blogsCount);
        }

        [Fact]
        public async Task Get_Blogs_CountWhere()
        {
            // Arrange

            // Act
            var blogsCount = await _blogsRepository.CountWhere(x => x.Posts.Count(y => y.Tags.Count == 2) > 4);

            // Assert
            Assert.Equal(1, blogsCount);
        }

        [Fact]
        public async Task Get_Blogs_Any()
        {
            // Arrange

            // Act
            var hasAny = await _blogsRepository.Any(x => x.Posts.Count(y => y.Tags.Count == 2) > 4);

            // Assert
            Assert.True(hasAny);
        }

        [Fact]
        public async Task Get_Blog()
        {
            // Arrange

            // Act
            var blog = await _blogsRepository.Find(1);

            // Assert
            Assert.Equal(1, blog.Id);
        }

        [Fact]
        public async Task Get_Blog_Children()
        {
            // Arrange

            // Act
            var blog = await _blogsRepository.Find(1);

            // Assert
            Assert.Equal(3, blog.Tags.Count);
            Assert.Equal(21, blog.Posts.Count);
        }

        [Fact]
        public async Task Get_Blog_Including()
        {
            // Arrange

            // Act
            var blog = await _blogsRepository.FindIncluding(1, true, x => x.Tags);

            // Assert
            Assert.Equal(3, blog.Tags.Count);
            Assert.Equal(0, blog.Posts.Count);
        }

        [Fact]
        public async Task Get_Blog_IncludingNone()
        {
            // Arrange

            // Act
            var blog = await _blogsRepository.FindIncluding(1);

            // Assert
            Assert.Equal(0, blog.Tags.Count);
            Assert.Equal(0, blog.Posts.Count);
        }

        [Fact]
        public async Task Get_Blogs()
        {
            // Arrange

            // Act
            var blogs = await _blogsRepository.FindAll();

            // Assert
            Assert.Equal(4, blogs.Count());
        }

        [Fact]
        public async Task Get_BlogsWhere()
        {
            // Arrange

            // Act
            var blogs = await _blogsRepository.FindAll(x => x.Id > Utilities.Utilities.BlogsCount - 2);

            // Assert
            Assert.Equal(2, blogs.Count());
        }
    }

    public class EditBlogTests
    {
        private static BlogContext GetNewContext(string databaseName)
        {
            var context = new BlogContext(new DbContextOptionsBuilder<DbContext>()
                .UseSqlite($"DataSource={databaseName};mode=memory;")
                .Options);

            context.Database.OpenConnection();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Utilities.Utilities.InitializeDbForTests(context);

            return context;
        }


        [Fact]
        public async Task Change_Name()
        {
            // Arrange
            await using var context = GetNewContext("Change_Name");
            var blogsRepository = new EntityRepository<Blog>(context);

            var blog = await blogsRepository.Find(1);
            blog.Name += " Modified";

            // Act
            var count = await blogsRepository.Update(blog);

            // Assert
            var modifiedBlog = await blogsRepository.Find(1);

            Assert.Equal(1, count);
            Assert.Equal("A1 Modified", modifiedBlog.Name);
        }

        [Fact]
        public async Task Add_Tag()
        {
            // Arrange
            await using var context = GetNewContext("Add_Tag");
            var blogsRepository = new EntityRepository<Blog>(context);

            var blog = await blogsRepository.Find(1);

            // Act
            blog.Tags.Add(new Tag { Name = "New Tag" });
            var count = await blogsRepository.Update(blog);

            // Assert
            var modifiedBlog = await blogsRepository.Find(1);

            Assert.Equal(2, count);
            Assert.Equal(4, modifiedBlog.Tags.Count);
            Assert.Contains("New Tag", modifiedBlog.Tags.Select(x => x.Name));
        }

        [Fact]
        public async Task Remove_Tag()
        {
            // Arrange
            await using var context = GetNewContext("Remove_Tag");
            var blogsRepository = new EntityRepository<Blog>(context);

            var blog = await blogsRepository.Find(1);
            var tag = blog.Tags.FirstOrDefault(x => x.Name == "T5");

            // Act
            blog.Tags.Remove(tag);
            var count = await blogsRepository.Update(blog);

            // Assert
            var modifiedBlog = await blogsRepository.Find(1);

            Assert.Equal(1, count);
            Assert.Equal(2, modifiedBlog.Tags.Count);
            Assert.DoesNotContain("T5", modifiedBlog.Tags.Select(x => x.Name));
        }
    }
}
