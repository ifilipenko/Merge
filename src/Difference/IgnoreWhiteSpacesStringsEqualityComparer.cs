using System.Collections.Generic;

namespace Merge
{
    internal class IgnoreWhiteSpacesStringsEqualityComparer : IEqualityComparer<Line>
    {
        public bool Equals(Line line1, Line line2)
        {
            return (ReferenceEquals(line2.Entry, null) && ReferenceEquals(line1.Entry, null)) ||
                   (!ReferenceEquals(line2.Entry, null) && !ReferenceEquals(line1.Entry, null) &&
                    Equals(line1.Entry.Trim(), line2.Entry.Trim()));
        }

        public int GetHashCode(Line obj)
        {
            return obj == null ? 0 : obj.Entry.GetHashCode();
        }
    }
}