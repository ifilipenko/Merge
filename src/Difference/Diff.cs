using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;

namespace Merge
{
    public class Diff
    {
        public static Diff Merge(Diff diff1, Diff diff2)
        {
            if (diff1._original != diff2._original)
                throw new ArgumentException("Diffs assigned with different original sequences");

            var mergedDiff = new Diff
                {
                    _original = diff1._original,
                    _ranges = MergeRanges(diff1, diff2)
                };

            return mergedDiff;
        }

        private static DifferenceRange[] MergeRanges(Diff diff1, Diff diff2)
        {
            var mergedRanges = diff1._ranges
                                    .Select(x => new
                                        {
                                            Source = diff1._ranges,
                                            Range = x
                                        })
                                    .Concat(diff2._ranges.Select(x => new
                                        {
                                            Source = diff2._ranges,
                                            Range = x
                                        }))
                                    .OrderBy(x => x.Range.From)
                                    .ToList();

            var result = new List<DifferenceRange>(mergedRanges.Count);
            for (int i = 0; i < mergedRanges.Count; i++)
            {
                var range = mergedRanges[i];
                if (i > 0)
                {
                    var prevRange = mergedRanges[i - 1];
                    if (range.Range.IsCrossed(prevRange.Range) && !ReferenceEquals(prevRange.Source, range.Source))
                    {
                        var lastMergedRange = result.Last();
                        var conflictedRange = lastMergedRange.CutRangeFrom(range.Range.From);
                        if (lastMergedRange.Length == 0)
                        {
                            result.Remove(lastMergedRange);
                        }

                        var currentRange = new DifferenceRange(range.Range);
                        var conflictedPart = currentRange.CutRangeTo(prevRange.Range.To, conflictedRange);
                        result.Add(conflictedPart);
                        if (currentRange.Length > 0)
                        {
                            result.Add(currentRange);
                        }

                        continue;
                    }
                }
                result.Add(new DifferenceRange(range.Range));
            }
            return result.ToArray();
        }

        private string[] _original;
        private DifferenceRange[] _ranges;

        private Diff()
        {
        }

        public Diff(string[] original, string[] target)
        {
            if (original == null)
                throw new ArgumentNullException("original");
            if (target == null)
                throw new ArgumentNullException("target");

            _original = original;

            var originalLines = original.Select((x, idx) => new Line(idx, x)).ToArray();
            var targetLines = target.Select((x, idx) => new Line(idx, x)).ToArray();
            if (originalLines.Length == 0 && targetLines.Length == 0)
            {
                _ranges = new DifferenceRange[0];
            }
            else if (originalLines.Length == 0)
            {
                _ranges = DifferenceRange.NewLinesRange(targetLines).ToEnumerable().ToArray();
            }
            else if (targetLines.Length == 0)
            {
                _ranges = DifferenceRange.DeletedLinesRange(0, original.Length - 1).ToEnumerable().ToArray();
            }
            else
            {
                var rangesBuilder = new DifferenceAlgorithm(originalLines, targetLines);
                _ranges = rangesBuilder.FindDiffs().ToArray();
            }
        }

        public DifferenceRange[] Ranges
        {
            get { return _ranges; }
        }

        public bool HasConflicts
        {
            get { return _ranges.Any(x => x.HasConflict); }
        }

        public string[] PatchOriginal()
        {
            if (_ranges.Length == 0)
                return _original.ToArray();

            var result = _original.ToList();
            var offset = 0;
            foreach (var range in _ranges)
            {
                switch (range.DifferenceType)
                {
                    case DifferenceType.Delete:
                        result.RemoveRange(range.From + offset, range.Length);
                        offset -= range.Length;
                        break;
                    case DifferenceType.Add:
                        result.InsertRange(range.From + offset, range.AddedLines.Select(x => x.Entry));
                        offset += range.Length;
                        break;
                }
            }
            return result.ToArray();
        }

        public Difference[] GetDiffPerLine()
        {
            var originalLines = _original.Select((x, idx) => new Line(idx, x)).ToArray();
            if (_ranges.Length == 0)
            {
                return originalLines.Select((x, i) => Difference.Equal(x)).ToArray();
            }

            var result = originalLines.Select((x, i) => Difference.Equal(x)).Take(_ranges[0].From).ToList();
            var nextEqualFrom = result.Count;
            for (int i = 0; i < _ranges.Length; i++)
            {
                var range = _ranges[i];
                switch (range.DifferenceType)
                {
                    case DifferenceType.Delete:
                        var deletedLines = originalLines.Skip(range.From).Take(range.Length).Select(Difference.Delete);
                        result.AddRange(deletedLines);
                        nextEqualFrom = range.To + 1;
                        break;
                    case DifferenceType.Add:
                        result.AddRange(range.AddedLines.Select(Difference.Added));
                        break;
                }

                var nextRange = i + 1 < _ranges.Length ? _ranges[i + 1] : null;
                int equalLength;
                if (nextRange == null)
                {
                    equalLength = originalLines.Length - nextEqualFrom;
                }
                else
                {
                    if (nextRange.From == range.From)
                        continue;
                    equalLength = _ranges[i + 1].From - nextEqualFrom;
                }

                var equalLines = originalLines.Skip(nextEqualFrom).Take(equalLength).Select(x => Difference.Equal(x));
                result.AddRange(equalLines);
                nextEqualFrom++;
            }
            return result.ToArray();
        }
    }
}