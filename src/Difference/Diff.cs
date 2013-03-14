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
                    _ranges = MergeRanges(diff1, diff2).ToArray()
                };

            return mergedDiff;
        }

        private static IEnumerable<DifferenceRange> MergeRanges(Diff diff1, Diff diff2)
        {
            var mergedRanges = diff1._ranges
                                    .Select(x => new {Source = diff1._ranges, Range = x})
                                    .Concat(diff2._ranges.Select(x => new {Source = diff2._ranges, Range = x}))
                                    .OrderBy(x => x.Range.From)
                                    .ToList();

            if (mergedRanges.Count == 1)
                yield return mergedRanges.First().Range;

            int lastReturnedRange = mergedRanges.Count;
            int i = 1;
            while (i < mergedRanges.Count)
            {
                var range     = mergedRanges[i];
                var prevRange = mergedRanges[i - 1];
                if (range.Range.IsCrossed(prevRange.Range) && !ReferenceEquals(prevRange.Source, range.Source))
                {
                    var splittingRange = prevRange.Range.CutRange(range.Range.From, range.Range.To);
                    splittingRange.CuttedRange.MakeConflictedWith(range.Range.Clone());
                    if (splittingRange.Before != null)
                        yield return splittingRange.Before;
                    yield return splittingRange.CuttedRange;
                    if (splittingRange.After != null)
                        yield return splittingRange.After;

                    lastReturnedRange = i;
                    i += 2;
                }
                else
                {
                    yield return prevRange.Range.Clone();
                    lastReturnedRange = i - 1;
                    i++;
                }
            }

            var trailRangeIndex = lastReturnedRange + 1;
            if (trailRangeIndex < mergedRanges.Count)
            {
                yield return mergedRanges[trailRangeIndex].Range.Clone();
            }
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
                if (range.HasConflict)
                {
                    var conflictedLines = GetConflictedLines(range);
                    result.InsertRange(range.From + offset, conflictedLines);
                    offset += conflictedLines.Count;
                }
                else
                {
                    switch (range.DifferenceType)
                    {
                        case DifferenceType.Delete:
                            result.RemoveRange(range.From + offset, range.Length);
                            offset -= range.Length;
                            break;
                        case DifferenceType.Replace:
                            result.RemoveRange(range.From + offset, range.Length);
                            result.InsertRange(range.From + offset, range.AddedLines.Select(x => x.Entry));
                            break;
                        case DifferenceType.Add:
                            result.InsertRange(range.From + offset, range.AddedLines.Select(x => x.Entry));
                            offset += range.Length;
                            break;
                    }
                }
            }
            return result.ToArray();
        }

        private IList<string> GetConflictedLines(DifferenceRange range)
        {
            var result = new List<string>();

            result.Add("<<<");
            result.AddRange(GetRangeChanges(range));
            result.Add("---");
            result.AddRange(GetRangeChanges(range.ConflictedWith));
            result.Add(">>>");

            return result;
        }

        private IEnumerable<string> GetRangeChanges(DifferenceRange range)
        {
            switch (range.DifferenceType)
            {
                case DifferenceType.Delete:
                    for (int i = range.From; i < range.ConflictedWith.To; i++)
                    {
                        yield return "-" + _original[i];
                    }
                    break;
                case DifferenceType.Replace:
                case DifferenceType.Add:
                    foreach (var addedLine in range.AddedLines)
                    {
                        yield return addedLine.Entry;
                    }
                    break;
            }
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
                    case DifferenceType.Replace:
                        result.AddRange(range.AddedLines.Select(Difference.Replaced));
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

                var equalLines = originalLines.Skip(nextEqualFrom).Take(equalLength).Select(Difference.Equal);
                result.AddRange(equalLines);
                if (equalLength > 0)
                {
                    nextEqualFrom++;
                }
            }
            return result.ToArray();
        }
    }
}