using System;
using System.Linq;

namespace Merge
{
    public class Diff
    {
        public Difference[] GetLinesDifference(string[] original, string[] target)
        {
            if (original == null)
                throw new ArgumentNullException("original");
            if (target == null)
                throw new ArgumentNullException("target");

            var originalLines = original.Select((x, idx) => new Line(idx, x, original)).ToArray();
            var targetLines = target.Select((x, idx) => new Line(idx, x, target)).ToArray();
            if (originalLines.Length == 0)
            {
                return targetLines.Select(Difference.Added).ToArray();
            }

            if (targetLines.Length == 0)
            {
                return originalLines.Select(Difference.Delete).ToArray();
            }

            var lcs = new LargestCommonSubsequence<Line>(originalLines, targetLines);
            return lcs.Backtrack(processAdded: Difference.Added,
                                 processDeleted: Difference.Delete,
                                 processEquals: Difference.Equal)
                      .ToArray();
        }
    }
}