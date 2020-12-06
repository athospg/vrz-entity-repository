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
    public class GetPagedPostTests
    {
        private readonly BlogContext _context;
        private readonly IEntityPagingRepository<long, Post> _postsRepository;

        public GetPagedPostTests()
        {
            _context = new BlogContext();

            _context.Database.OpenConnection();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            Utilities.Utilities.InitializeDbForTests(_context);

            _postsRepository = new EntityPagingRepository<Post>(_context);
        }

        ~GetPagedPostTests()
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
}
