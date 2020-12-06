using System.Linq;
using VRZ.EntityRepository.SDK.Utils;
using Xunit;

namespace VRZ.EntityRepository.Tests.Unit.Utils
{
    public class RandomSplitCountsTests
    {
        [Fact]
        public void DefaultSeed()
        {
            // Arrange
            const int totalGroups = 5;
            const int totalSize = 30;

            // Act
            var groups = RandomUtils.RandomSplitCounts(totalGroups, totalSize).ToList();

            // Assert
            Assert.Equal(5, groups.Count);
        }

        [Fact]
        public void DefiningSeed()
        {
            // Arrange
            const int totalGroups = 5;
            const int totalSize = 30;

            // Act
            var groups = RandomUtils.RandomSplitCounts(totalGroups, totalSize, 7).ToList();

            // Assert
            Assert.Equal(new[] { 2, 10, 8, 6, 4 }, groups);
        }
    }
}
