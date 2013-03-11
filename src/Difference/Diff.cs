using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;

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
                return targetLines.Select(x => Difference.Added(null, x)).ToArray();
            }

            if (targetLines.Length == 0)
            {
                return originalLines.Select(x => Difference.Delete(x, null)).ToArray();
            }

            var originalRange = new LinesRange(originalLines, 0, originalLines.Length - 1);
            var targetRange   = new LinesRange(targetLines, 0, targetLines.Length - 1);
            return RangeDifference(originalRange, targetRange).ToArray();
        }

        private IEnumerable<Difference> RangeDifference(LinesRange original, LinesRange target)
        {
            if (original.Length <= 1 && target.Length <= 1)
            {
                var originalLine = original.Length > 0 ? original[0] : null;
                var targetLine   = target.Length > 0 ? target[0] : null;
                return DifferenceForLines(originalLine, targetLine).ToEnumerableOrEmpty();
            }

            var originalParts = original.HalfParts();
            var targetParts   = target.HalfParts();

            return RangeDifference(originalParts[0], targetParts[0])
                .Concat(RangeDifference(originalParts[1], targetParts[1]));
        }

       private static Difference DifferenceForLines(Line originalLine, Line targetLine)
        {
            if (originalLine == null && targetLine == null)
            {
                return null;
            }
            if (originalLine == null)
            {
                return Difference.Added(null, targetLine);
            }
            if (targetLine == null)
            {
                return Difference.Delete(originalLine, null);
            }
            if (originalLine.Equals(targetLine))
            {
                return Difference.Equal(originalLine, targetLine);
            }
            return Difference.Changed(originalLine, targetLine);
        }

       /* private IEnumerable<Difference> FindDifferences(string[] original, string[] target)
       {
           var differences = new List<Difference>();
           var longestLength = original.Length > target.Length ? original.Length : target.Length;
           var shortestLength = original.Length < target.Length ? original.Length : target.Length;
           var line = 0;
           int iOriginalLine;
           int iTargetLine;
           var result = new List<Difference>();

           var diffPosition = new DiffPosition();
           result.Add(ScanDifferences(differences, original, target, ref diffPosition));
           result.Add(TargetTrailDifferences(original, target, diffPosition));
           result.Add(OriginalTrailDifferences(original, target, diffPosition));

           return result;
       }

       private void ScanDifferences(List<Difference> differences, string[] original, string[] target, ref DiffPosition diffPosition)
       {
           while (diffPosition.OriginalLineIndex < original.Length && diffPosition.TargetLineIndex < target.Length)
           {
               var originalLine = original[diffPosition.OriginalLineIndex];
               var targetLine = target[diffPosition.TargetLineIndex];

               if (!AreEqual(originalLine, targetLine))
               {
                    
                   FindAddedLines(differences, original, target, diffPosition, originalLine, targetLine);
               }
           }
       }

       private void FindAddedLines(List<Difference> differences, string[] original, string[] target, int iOriginalLine, int iTargetLine,
                                  string originalLine, string targetLine)
       {
           var nextOriginalLine = NextLine(original, iOriginalLine);
           var newTargetLine = iTargetLine;
           while (nextOriginalLine != null && !AreEqual(originalLine, targetLine))
           {
               var nextTargetLine = target[newTargetLine + 1];
               if (nextTargetLine == nextOriginalLine)
               {
                   differences.Add(Difference.Changed(originalLine, targetLine));
                   break;
               }
               newTargetLine++;
               targetLine = nextTargetLine;
           }
           while (iTargetLine < newTargetLine - 1)
           {
               differences.Add(Difference.Added(null, target[iTargetLine++]));
           }
           return iTargetLine;
       }

       private static IEnumerable<Difference> OriginalTrailDifferences(string[] original, string[] target, int iTargetLine,
                                                           int iOriginalLine)
       {
           if (iTargetLine >= target.Length)
           {
               while (iOriginalLine < target.Length)
               {
                   yield return Difference.Delete(original[iOriginalLine++], null);
               }
               yield break;
           }
       }

       private static IEnumerable<Difference> TargetTrailDifferences(string[] original, string[] target, int iOriginalLine, int iTargetLine)
       {
           if (iOriginalLine >= original.Length)
           {
               while (iTargetLine < target.Length)
               {
                   yield return Difference.Added(null, target[iTargetLine++]);
               }
               yield break; // break;
           }
       }

       private bool AreEqual(string originalLine, string targetLine)
       {
           return Equals(originalLine.Trim(), targetLine.Trim());
       }

       private string NextLine(string[] lines, int iLine)
       {
           var iNextLine = iLine+1;
           return iNextLine < lines.Length ? lines[iNextLine] : null;
       }*/

        class LinesRange
        {
            private readonly Line[] _lines;

            public LinesRange(Line[] lines, int from, int to)
            {
                _lines = lines;
                From = @from;
                To = to;
            }
            
            public int From { get; private set; }
            public int To { get; private set; }

            public int Length
            {
                get { return To - From + 1; }
            }

            public LinesRange[] HalfParts()
            {
                var half = Length/2;
                var toFirst = From + half - 1;
                return new[]
                    {
                        new LinesRange(_lines, From, toFirst),
                        new LinesRange(_lines, toFirst + 1, To)
                    };
            }

            public Line this[int i]
            {
                get { return _lines[i + From]; }
            }

            public override string ToString()
            {
                return string.Format("{0} - {1} (Length: {2})", From, To, Length);
            }
        }

        struct DiffPosition
        {
            public int OriginalLineIndex;
            public int TargetLineIndex;
        }
    }
}