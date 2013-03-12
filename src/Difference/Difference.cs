using System;

namespace Merge
{
    public class Difference
    {
        public static Difference Delete(Line line)
        {
            return new Difference(line, null, DifferenceType.Deleted);
        }

        public static Difference Added(Line line)
        {
            return new Difference(null, line, DifferenceType.Added);
        }

        public static Difference Equal(Line text1, Line text2)
        {
            return new Difference(text1, text2, DifferenceType.Equals);
        }

        public static Difference Changed(Line text1, Line text2)
        {
            return new Difference(text1, text2, DifferenceType.Changed);
        }

        public Difference(Line line1, Line line2, DifferenceType type)
        {
            Line1 = line1;
            Line2 = line2;
            Type = type;
        }

        public Line Line1 { get; set; }
        public Line Line2 { get; set; }
        public DifferenceType Type { get; set; }

        public override string ToString()
        {
            var sourceIndex = Line1 == null ? string.Empty : "source: " + Line1.Index;
            var targetIndex = Line2 == null ? string.Empty : "target: " + Line2.Index;
            return sourceIndex + ", " + targetIndex + " [" + DifferenceString() + "]";
        }

        private string DifferenceString()
        {
            switch (Type)
            {
                case DifferenceType.Equals:
                    return Line1.ToString();
                case DifferenceType.Deleted:
                    return "-" + Line1;
                case DifferenceType.Added:
                    return "+" + Line2;
                case DifferenceType.Changed:
                    return Line1 + Environment.NewLine + " >> " + Line2;
                default:
                    return string.Empty;
            }
        }
    }
}