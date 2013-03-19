namespace Merge
{
    public class Line
    {
        public Line(int index, string entry)
        {
            Index = index;
            Entry = entry ?? string.Empty;
        }

        public int Index { get; private set; }
        public string Entry { get; private set; }

        public override string ToString()
        {
            return Entry;
        }

    }
}