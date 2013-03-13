using System.Collections.Generic;

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
            lcs.Backtrack(processAdded: line => ProcessDiff(line, DifferenceType.Added),
                          processDeleted: line => ProcessDiff(line, DifferenceType.Deleted),
                          processEquals: (line1, line2) => _lastDiffType = null);

            return _ranges;
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