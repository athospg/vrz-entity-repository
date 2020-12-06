using System;
using System.Collections.Generic;
using System.Linq;
using VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities.Models;
using VRZ.Infrastructure.Utils;

namespace VRZ.EntityRepository.Tests.Integration.BlogContextTests.Utilities
{
    public static class Utilities
    {
        public const int TagsCount = 5;
        public const int BlogsCount = 4;
        public const int PostsCount = 75;

        public static async void InitializeDbForTests(BlogContext db)
        {
            if (db.Tags.Any())
                return;

            IEntityRepository<long, Tag> tagsRepo = new EntityRepository<Tag>(db);
            IEntityRepository<long, Blog> blogsRepository = new EntityRepository<Blog>(db);
            IEntityRepository<long, Post> postsRepository = new EntityRepository<Post>(db);

            var tags = GetSeedingTags();

            await tagsRepo.Add(tags);
            await blogsRepository.Add(GetSeedingBlogs(tags));
            await postsRepository.Add(GetSeedingPosts(tags));
        }

        public static void ReinitializeDbForTests(BlogContext db)
        {
            db.Tags.RemoveRange(db.Tags);

            db.Posts.RemoveRange(db.Posts);
            db.Blogs.RemoveRange(db.Blogs);

            InitializeDbForTests(db);
        }


        private static List<Tag> GetSeedingTags()
        {
            var values = new List<Tag>();
            for (var i = 1; i <= TagsCount; i++)
            {
                values.Add(new Tag
                {
                    Id = i,
                    Name = $"T{i:D2}",
                });
            }

            return values;
        }

        private static IEnumerable<Blog> GetSeedingBlogs(IReadOnlyCollection<Tag> allTags)
        {
            var rand = new Random(0);
            var values = new List<Blog>();
            for (var i = 1; i <= BlogsCount; i++)
            {
                values.Add(new Blog
                {
                    Id = i,
                    Name = $"A{i:D2}",
                    Tags = GetRandomItems(allTags, rand),
                });
            }

            return values;
        }

        private static IEnumerable<Post> GetSeedingPosts(IReadOnlyCollection<Tag> allTags)
        {
            var rand = new Random(0);
            var values = new List<Post>();

            var postId = 1;
            var initialData = new DateTime(2020, 1, 1);

            var blogsPostCount = RandomUtils.RandomSplitCounts(BlogsCount, PostsCount, 7).ToList();
            for (var i = 1; i <= BlogsCount; i++)
            {
                var postsCount = blogsPostCount[i - 1];

                for (var j = 1; j <= postsCount; j++)
                {
                    values.Add(new Post
                    {
                        Id = postId++,
                        Date = initialData.AddHours(j),
                        Name = $"Blog {i:D2} Post {j:D2}",
                        Tags = GetRandomItems(allTags, rand),
                        BlogId = i,
                    });
                }
            }

            return values;
        }


        private static List<T> GetRandomItems<T>(IEnumerable<T> values, Random rand)
        {
            var valuesCopy = new List<T>(values);
            var items = new List<T>();

            var count = rand.Next(0, valuesCopy.Count);
            Console.WriteLine($"{count}");

            while (items.Count < count)
            {
                var index = rand.Next(0, valuesCopy.Count);
                var item = valuesCopy[index];

                valuesCopy.RemoveAt(index);
                items.Add(item);
            }

            return items;
        }
    }
}
