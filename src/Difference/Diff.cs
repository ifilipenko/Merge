using System;
using System.Collections.Generic;
using System.Linq;

namespace Merge
{
    public struct DifferenceRange
    {
        private readonly DifferenceType _differenceType;
        private readonly int _from;
        private readonly int _to;

        public DifferenceRange(DifferenceType differenceType, int from, int to)
        {
            _differenceType = differenceType;
            _from = @from;
            _to = to;
        }

        public DifferenceType DifferenceType
        {
            get { return _differenceType; }
        }

        public int From
        {
            get { return _from; }
        }

        public int To
        {
            get { return _to; }
        }

        public DifferenceRange ExtendRangeToIndex(int index)
        {
            return new DifferenceRange(_differenceType, _from, index);
        }

        public IEnumerable<Difference> GetDifference(string[] original, string[] target)
        {
            throw new NotImplementedException();
        }
    }

    public class DifferenceMap
    {
        private readonly string[] _original;
        private readonly string[] _target;
        private DifferenceRange[] _differences;

        public DifferenceMap(string[] original, string[] target)
        {
            if (original == null)
                throw new ArgumentNullException("original");
            if (target == null)
                throw new ArgumentNullException("target");

            _original = original;
            _target = target;

            var originalLines = original.Select((x, idx) => new Line(idx, x, original)).ToArray();
            var targetLines   = target.Select((x, idx) => new Line(idx, x, target)).ToArray();
            if (originalLines.Length == 0)
            {
                _differences = new[] {new DifferenceRange(DifferenceType.Added, 0, target.Length - 1)};
            }
            else if (targetLines.Length == 0)
            {
                _differences = new[] {new DifferenceRange(DifferenceType.Deleted, 0, original.Length - 1)};
            }
            else
            {
                DifferenceType? lastDiffType = null;
                var diffList = new List<DifferenceRange>();

                var lcs = new LargestCommonSubsequence<Line>(originalLines, targetLines);
                lcs.Backtrack(processAdded: line =>
                                                {
                                                    ProcessDiff(line, DifferenceType.Added, lastDiffType, diffList);
                                                    lastDiffType = DifferenceType.Added;
                                                },
                              processDeleted: line =>
                                                  {
                                                      ProcessDiff(line, DifferenceType.Deleted, lastDiffType, diffList);
                                                      lastDiffType = DifferenceType.Deleted;
                                                  },
                              processEquals: (line1, line2) => lastDiffType = DifferenceType.Equals);

                _differences = diffList.ToArray();
            }
        }

        private void ProcessDiff(Line line, DifferenceType differenceType, DifferenceType? previousDiffType, IList<DifferenceRange> diffList)
        {
            if (previousDiffType == differenceType && diffList.Count > 0)
            {
                var lastRange = diffList[diffList.Count - 1];
                if (lastRange.DifferenceType == differenceType)
                {
                    diffList[diffList.Count - 1] = lastRange.ExtendRangeToIndex(line.Index);
                    return;
                }
            }
            diffList.Add(new DifferenceRange(differenceType, line.Index, line.Index));
        }

        public Difference[] GetFullDifference()
        {
            return _differences.SelectMany(d => d.GetDifference(_original, _target)).ToArray();
        }

        public DifferenceRange[] Differences
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    public static class Diff
    {
        public static Difference[] GetLinesDifference(string[] original, string[] target)
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