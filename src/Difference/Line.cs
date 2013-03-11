using System;

namespace Merge
{
    public class Line : IEquatable<Line>
    {
        public Line(int index, string entry, string[] source)
        {
            Index = index;
            Entry = entry;
            Source = source;
        }

        public string[] Source { get; private set; }
        public int Index { get; private set; }
        public string Entry { get; private set; }

        public override bool Equals(object obj)
        {
            return Equals((Line)obj);
        }

        public bool Equals(Line line)
        {
            return (ReferenceEquals(line.Entry, null) && ReferenceEquals(Entry, null)) ||
                   (!ReferenceEquals(line.Entry, null) && !ReferenceEquals(Entry, null) &&
                    Equals(Entry.Trim(), line.Entry.Trim()));
        }

        public override string ToString()
        {
            return Entry;
        }

    }
}