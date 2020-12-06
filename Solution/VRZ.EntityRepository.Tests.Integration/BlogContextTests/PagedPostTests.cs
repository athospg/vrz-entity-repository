using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VRZ.EntityRepository.Paging;
using VRZ.EntityRepository.Paging.Filters;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models;
using Xunit;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests
{
    public class GetPagedPostsTests
    {
        private readonly BlogContext _context;
        private readonly IEntityPagingRepository<long, Post> _postsRepository;

        public GetPagedPostsTests()
        {
            _context = new BlogContext();

            _context.Database.OpenConnection();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            Utilities.Utilities.InitializeDbForTests(_context);

            _postsRepository = new EntityPagingRepository<Post>(_context);
        }

        ~GetPagedPostsTests()
        {
            _context.Dispose();
        }


        [Fact]
        public async Task Get_Posts_TotalCount()
        {
            // Arrange
            var filter = new PagingFilter();

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(Utilities.Utilities.PostsCount, posts.TotalCount);
        }

        [Fact]
        public async Task Get_Posts_PageSize()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageSize = 23,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(23, posts.PageSize);
        }

        [Fact]
        public async Task Get_Posts_PageSize_Negative()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageSize = -5,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(10, posts.PageSize);
        }

        [Fact]
        public async Task Get_Posts_MaxPageSize()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageSize = 23,
                MaxPageSize = 15,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(15, posts.PageSize);
        }

        [Fact]
        public async Task Get_Posts_MaxPageSize_Negative()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageSize = 23,
                MaxPageSize = -5,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(23, posts.PageSize);
        }

        [Fact]
        public async Task Get_Posts_MaxPageSize_NegativeAfterPositive()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageSize = 23,
                MaxPageSize = 15,
            };

            // Act
            filter.MaxPageSize = -5;
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(15, posts.PageSize);
        }

        [Fact]
        public async Task Get_Posts_TotalPagesCount()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageSize = 15,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal((int)Math.Ceiling(Utilities.Utilities.PostsCount / 15d), posts.TotalPages);
        }

        [Fact]
        public async Task Get_Posts_HasPreviousPage()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageNumber = 2,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.True(posts.HasPrevious);
        }

        [Fact]
        public async Task Get_Posts_DoNotHavePreviousPage()
        {
            // Arrange
            var filter = new PagingFilter();

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.False(posts.HasPrevious);
        }

        [Fact]
        public async Task Get_Posts_HasNextPage()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageNumber = 2,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.True(posts.HasNext);
        }

        [Fact]
        public async Task Get_Posts_DoNotHaveNextPage()
        {
            // Arrange
            const int pageSize = 17;
            var lastPage = (int)Math.Ceiling(Utilities.Utilities.PostsCount / (double)pageSize);
            var filter = new PagingFilter
            {
                PageSize = pageSize,
                PageNumber = lastPage,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.False(posts.HasNext);
        }

        [Fact]
        public async Task Get_Posts_ResultCount()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageSize = 23,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(23, posts.Count);
        }

        [Fact]
        public async Task Get_Posts_ResultCount_AfterLastPage()
        {
            // Arrange
            const int pageSize = 17;
            var lastPage = (int)Math.Ceiling(Utilities.Utilities.PostsCount / (double)pageSize);
            var filter = new PagingFilter
            {
                PageSize = pageSize,
                PageNumber = lastPage + 1,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Empty(posts);
        }

        [Fact]
        public async Task Get_Posts_PageNumber()
        {
            // Arrange
            const int pageSize = 17;
            var lastPage = (int)Math.Ceiling(Utilities.Utilities.PostsCount / (double)pageSize);
            var filter = new PagingFilter
            {
                PageSize = pageSize,
                PageNumber = lastPage - 1,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(pageSize, posts.Count);
        }

        [Fact]
        public async Task Get_Posts_PageNumber_Negative()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageNumber = -1,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(1, posts.CurrentPage);
        }

        [Fact]
        public async Task Get_Posts_PageNumber_AfterLastPage()
        {
            // Arrange
            const int pageSize = 17;
            var lastPage = (int)Math.Ceiling(Utilities.Utilities.PostsCount / (double)pageSize);
            var filter = new PagingFilter
            {
                PageSize = pageSize,
                PageNumber = lastPage + 1,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal(lastPage + 1, posts.CurrentPage);
        }


        [Fact]
        public async Task Get_Posts_Including()
        {
            // Arrange
            var filter = new PagingFilter
            {
                OrderBy = nameof(Post.Name),
            };

            // Act
            var posts = await _postsRepository.FindAllIncluding(filter, true, x => x.Tags);

            // Assert
            Assert.Contains(posts, x => x.Tags.Count > 0);
        }

        [Fact]
        public async Task Get_Post_Including()
        {
            // Arrange
            var filter = new PagingFilter();

            // Act
            var posts = await _postsRepository.FindAllIncluding(filter, x => x.Id == 1, true, x => x.Tags);

            // Assert
            Assert.Equal(3, posts.First().Tags.Count);
        }

        [Fact]
        public async Task Get_Post_IncludingNone()
        {
            // Arrange
            var filter = new PagingFilter();

            // Act
            var posts = await _postsRepository.FindAllIncluding(filter, x => x.Id == 1);

            // Assert
            Assert.Equal(0, posts.First().Tags.Count); ;
        }

        [Fact]
        public async Task Get_PostsWhere()
        {
            // Arrange
            var filter = new PagingFilter();

            // Act
            var posts = await _postsRepository.FindAll(filter, x => x.Id > Utilities.Utilities.PostsCount - 2);

            // Assert
            Assert.Equal(2, posts.Count);
        }

        [Fact]
        public async Task Get_PostsWhereChildren()
        {
            // Arrange
            var filter = new PagingFilter();

            // Act
            var posts = await _postsRepository.FindAllIncluding(filter, x => x.Tags.Count > 3, true, x => x.Tags);

            // Assert
            Assert.Equal(10, posts.Count);
        }
    }

    public class GetPagedPostsContentTests
    {
        private readonly BlogContext _context;
        private readonly IEntityPagingRepository<long, Post> _postsRepository;

        public GetPagedPostsContentTests()
        {
            _context = new BlogContext();

            _context.Database.OpenConnection();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            Utilities.Utilities.InitializeDbForTests(_context);

            _postsRepository = new EntityPagingRepository<Post>(_context);
        }

        ~GetPagedPostsContentTests()
        {
            _context.Dispose();
        }


        [Fact]
        public async Task Get_Posts_DefaultOrder()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageNumber = 2,
                PageSize = 2,
                Ascending = true,
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal("Blog 01 Post 27", posts.First().Name);
            Assert.Equal("Blog 01 Post 26", posts.Last().Name);
        }


        [Fact]
        public async Task Get_Posts_CustomOrder()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageNumber = 2,
                PageSize = 2,
                OrderBy = nameof(Post.Name),
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal("Blog 01 Post 03", posts.First().Name);
            Assert.Equal("Blog 01 Post 04", posts.Last().Name);
        }

        [Fact]
        public async Task Get_Posts_CustomOrder_Descending()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageNumber = 2,
                PageSize = 2,
                OrderBy = $"{nameof(Post.Name)} desc, {nameof(Post.Name)} asc, a$5@4H56agT, {nameof(Post.Id)} desc",
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            Assert.Equal("Blog 04 Post 08", posts.First().Name);
            Assert.Equal("Blog 04 Post 07", posts.Last().Name);
        }

        [Fact]
        public async Task Get_Posts_CustomOrder_MultipleProperties()
        {
            // Arrange
            var filter = new PagingFilter
            {
                PageSize = 8,
                OrderBy = $"{nameof(Post.Date)}, {nameof(Post.Name)} asc",
            };

            // Act
            var posts = await _postsRepository.FindAll(filter);

            // Assert
            var expected = new[]
            {
                "Blog 01 Post 01", "Blog 02 Post 01", "Blog 03 Post 01", "Blog 04 Post 01",
                "Blog 01 Post 02", "Blog 02 Post 02", "Blog 03 Post 02", "Blog 04 Post 02",
            };

            Assert.Equal(expected, posts.Select(x => x.Name).ToList());
        }
    }
}
