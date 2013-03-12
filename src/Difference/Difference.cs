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

        private Difference(Line line1, Line line2, DifferenceType type)
        {
            Line1 = line1;
            Line2 = line2;
            Type = type;
        }

        public Line Line1 { get; set; }
        public Line Line2 { get; set; }
        public DifferenceType Type { get; set; }

        public int ChangedLineIndex
        {
            get
            {
                return Line1 != null
                           ? Line1.Index
                           : Line2 != null
                                 ? Line2.Index
                                 : -1;
            }
        }

        public string LineEntry
        {
            get
            {
                var line = Line1 ?? Line2;
                return line == null ? null : line.Entry;
            }
        }

        public override string ToString()
        {
            var prefix = string.Empty;
            if (Line1 != null)
            {
                prefix += Line1.Index;
            }
            if (Line2 != null)
            {
                if (Line1 != null)
                {
                    prefix += "->" +  Line2.Index;
                }
                else
                {
                    prefix += Line2.Index;
                }
            }
            return string.IsNullOrEmpty(prefix) ? DifferenceString() : prefix + "\t\t" + DifferenceString();
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
                default:
                    return string.Empty;
            }
        }
    }
}