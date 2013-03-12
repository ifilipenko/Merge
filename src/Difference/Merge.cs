using System;
using System.Collections.Generic;
using System.Linq;

namespace Merge
{
    public static class Merge
    {
        private class MergedLine
        {
            public static MergedLine NewLine(string line, int newLineIndex)
            {
                return new MergedLine(line, -1, newLineIndex);
            }

            public static MergedLine ExistsLine(string line, int originalLineIndex, int newLineIndex)
            {
                return new MergedLine(line, originalLineIndex, newLineIndex);
            }

            private MergedLine(string line, int originalLineIndex, int newLineIndex)
            {
                Line = line;
                OriginalLineIndex = originalLineIndex;
                NewLineIndex = newLineIndex;
            }

            public string Line { get; set; }
            public int OriginalLineIndex { get; set; }
            public int NewLineIndex { get; set; }
        }

        public static string Execute(string[] originalLines, string[] file1Lines, string[] file2Lines)
        {
            if (originalLines == null) 
                throw new ArgumentNullException("originalLines");
            if (file1Lines == null) 
                throw new ArgumentNullException("file1Lines");
            if (file2Lines == null) 
                throw new ArgumentNullException("file2Lines");

            var difference1 = Diff.GetLinesDifference(originalLines, file1Lines);
            var difference2 = Diff.GetLinesDifference(originalLines, file2Lines);

            var mergedLines = MergeFirst(originalLines, difference1);
            mergedLines = MergeSecond(mergedLines, difference2);

            return string.Join(Environment.NewLine, mergedLines.Select(x => x.Line ?? string.Empty));
        }

        private static IEnumerable<MergedLine> MergeSecond(IEnumerable<MergedLine> mergedLines, IEnumerable<Difference> differences)
        {
            var changedLines = GetChangedLinesMap(differences);
            int iLine = 0;
            foreach (var mergedLine in mergedLines)
            {
                Difference[] lineDifferences;
                if (changedLines.TryGetValue(mergedLine.OriginalLineIndex, out lineDifferences))
                {
                    var mergedLineReturned = false;
                    foreach (var lineDifference in lineDifferences)
                    {
                        switch (lineDifference.Type)
                        {
                            case DifferenceType.Deleted:
                                continue;
                            case DifferenceType.Added:
                                var newMergedLine = MergedLine.NewLine(lineDifference.LineEntry, ++iLine);
                                yield return newMergedLine;
                                iLine++;
                                if (!mergedLineReturned)
                                {
                                    mergedLineReturned = true;
                                    yield return mergedLine;
                                }
                                break;
                        }
                    }
                }
                else
                {
                    iLine++;
                    yield return mergedLine;
                }
                // ReSharper restore PossibleNullReferenceException
            }
        }

        private static IEnumerable<MergedLine> MergeFirst(string[] originalLines, IEnumerable<Difference> differences)
        {
            var changedLines = GetChangedLinesMap(differences);
            int iLine = 0;
            for (int i = 0; i < originalLines.Length; i++)
            {
                var originalLine = originalLines[i];

                Difference[] lineDifferences;
                if (changedLines.TryGetValue(i, out lineDifferences))
                {
                    foreach (var lineDifference in lineDifferences)
                    {
                        switch (lineDifference.Type)
                        {
                            case DifferenceType.Deleted:
                                continue;
                            case DifferenceType.Added:
                                var mergedLine = MergedLine.NewLine(lineDifference.LineEntry, ++iLine);
                                iLine++;
                                yield return mergedLine;
                                break;
                        }
                    }
                }
                else
                {
                    yield return MergedLine.ExistsLine(originalLine, i, iLine++);
                }
            }
        }

        private static Dictionary<int, Difference[]> GetChangedLinesMap(IEnumerable<Difference> difference)
        {
            return difference.Where(x => x.Type != DifferenceType.Equals)
                             .GroupBy(x => x.ChangedLineIndex)
                             .ToDictionary(x => x.Key, x => x.ToArray());
        }
    }
}