namespace Merge
{
    public class Difference
    {
        public enum TypeEnum
        {
            Equals,
            Deleted,
            Added,
            Replaced
        }

        public static Difference Delete(Line line)
        {
            return new Difference(line, TypeEnum.Deleted);
        }

        public static Difference Added(Line line)
        {
            return new Difference(line, TypeEnum.Added);
        }

        public static Difference Replaced(Line line)
        {
            return new Difference(line, TypeEnum.Replaced);
        }

        public static Difference Equal(Line line)
        {
            return new Difference(line, TypeEnum.Equals);
        }

        private Difference(Line line, TypeEnum type)
        {
            Line = line;
            Type = type;
        }

        public Line Line { get; set; }
        public TypeEnum Type { get; set; }

        public override string ToString()
        {
            return Line.Index + ".\t\t" + DifferenceString();
        }

        private string DifferenceString()
        {
            switch (Type)
            {
                case TypeEnum.Equals:
                    return "  |" + Line;
                case TypeEnum.Deleted:
                    return "- |" + Line;
                case TypeEnum.Added:
                    return "+ |" + Line;
                case TypeEnum.Replaced:
                    return "-+|" + Line;
                default:
                    return string.Empty;
            }
        }
    }
}