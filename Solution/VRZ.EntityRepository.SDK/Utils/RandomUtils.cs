using System;
using System.Collections.Generic;
using System.Linq;

namespace VRZ.EntityRepository.SDK.Utils
{
    public static class RandomUtils
    {
        public static IEnumerable<int> RandomSplitCounts(int totalGroups, int totalSize, int? seed = null)
        {
            var r = seed.HasValue ? new Random(seed.Value) : new Random();
            var a = Enumerable.Repeat<int?>(null, totalGroups - 1)
                .Select(x => r.Next(1, totalSize))
                .Concat(new[] { 0, totalSize })
                .OrderBy(x => x)
                .ToArray();

            return a.Skip(1).Select((x, i) => x - a[i]);
        }
    }
}
