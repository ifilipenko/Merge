using System.Collections.Generic;
using System.Linq;

namespace Merge
{
    class DifferenceAlgorithm
    {
        private List<DifferenceRange> _ranges;
        private readonly Line[] _originalLines;
        private readonly Line[] _targetLines;
        private DifferenceType? _lastDiffType;

        public DifferenceAlgorithm(Line[] originalLines, Line[] targetLines)
        {
            _originalLines = originalLines;
            _targetLines = targetLines;
        }

        public IEnumerable<DifferenceRange> FindDiffs()
        {
            _ranges = new List<DifferenceRange>();
            _lastDiffType = null;

            var lcs = new LargestCommonSubsequence<Line>(_originalLines, _targetLines, new IgnoreWhiteSpacesStringsEqualityComparer());
            lcs.Backtrack(processAdded: line => ProcessDiff(line, DifferenceType.Add),
                          processDeleted: line => ProcessDiff(line, DifferenceType.Delete),
                          processEquals: (line1, line2) => _lastDiffType = null);

            return GetReplacedDifferenceRanges().ToArray();
        }

        private IEnumerable<DifferenceRange> GetReplacedDifferenceRanges()
        {
            int i = 1;
            var prevRange = _ranges[0];
            while (i < _ranges.Count)
            {
                if (prevRange == null)
                {
                    prevRange = _ranges[i - 1];
                }
                var currRange = _ranges[i];
                if (prevRange.From == currRange.From && prevRange.Length <= currRange.Length &&
                    prevRange.DifferenceType == DifferenceType.Delete &&
                    currRange.DifferenceType == DifferenceType.Add)
                {
                    var partRange = currRange.CutRangeTo(prevRange.To);
                    partRange.MarkReplace();
                    yield return partRange;
                    if (currRange.Length != 0)
                    {
                        prevRange = currRange;
                        yield return currRange;
                    }
                    else
                    {
                        prevRange = null;
                        i++;
                    }
                }
                else
                {
                    if (i == 1)
                        yield return prevRange;
                    yield return currRange;
                    prevRange = null;
                }
                i++;
            }
        }

        private void ProcessDiff(Line line, DifferenceType differenceType)
        {
            var lastDiffType = _lastDiffType;
            _lastDiffType = differenceType;

            if (lastDiffType == differenceType && _ranges.Count > 0)
            {
                var lastRange = _ranges[_ranges.Count - 1];
                if (lastRange.DifferenceType == differenceType)
                {
                    _ranges[_ranges.Count - 1].ExtendRangeToLine(line);
                    return;
                }
            }
            _ranges.Add(new DifferenceRange(differenceType, line));
        }
    }
}