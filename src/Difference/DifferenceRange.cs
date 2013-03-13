using System;
using System.Collections.Generic;
using System.Linq;

namespace Merge
{
    

    public class DifferenceRange : IEquatable<DifferenceRange>
    {
        public static DifferenceRange NewLinesRange(IList<Line> lines)
        {
            var from = lines.First().Index;
            var to = lines.Last().Index;
            var range = new DifferenceRange(DifferenceType.Added, @from, to);
            range._addedLines.AddRange(lines);
            return range;
        }

        public static DifferenceRange DeletedLinesRange(int from, int to)
        {
            return new DifferenceRange(DifferenceType.Deleted, @from, to);
        }

        private readonly DifferenceType _differenceType;
        private readonly int _from;
        private int _to;
        private readonly DifferenceRange _conflictedWith;
        private readonly List<Line> _addedLines;

        public DifferenceRange(DifferenceRange original, DifferenceRange conflictedWith = null)
            : this(original.DifferenceType, original.From, original.To, conflictedWith)
        {
            if (original._addedLines.Count > 0)
            {
                _addedLines.AddRange(original._addedLines);
            }
        }

        public DifferenceRange(DifferenceType differenceType, Line line, DifferenceRange conflictedWith = null)
            : this(differenceType, line.Index, line.Index, conflictedWith)
        {
            ProcessLine(line);
        }

        private DifferenceRange(DifferenceType differenceType, int from, int to, DifferenceRange conflictedWith = null)
        {
            _addedLines = new List<Line>(0);
            _differenceType = differenceType;
            _from = @from;
            SetToIndex(to);
            _conflictedWith = conflictedWith;
        }

        public DifferenceType DifferenceType
        {
            get { return _differenceType; }
        }

        public int From
        {
            get { return _from; }
        }

        public int To
        {
            get { return _to; }
        }

        public int Length
        {
            get { return _to - _from + 1; }
        }

        public IEnumerable<Line> AddedLines
        {
            get { return _addedLines.AsReadOnly(); }
        }

        public DifferenceRange ConflictedWith
        {
            get { return _conflictedWith; }
        }

        public bool HasConflict
        {
            get { return _conflictedWith != null; }
        }

        public void ExtendRangeToLine(Line line)
        {
            SetToIndex(line.Index);
            ProcessLine(line);
        }

        public bool IsCrossed(DifferenceRange other)
        {
            return (From == other.From && other.To == To) ||
                   (From <= other.From && other.From <= To) ||
                   (To >= other.From && To <= other.To);
        }

        private void ProcessLine(Line line)
        {
            if (DifferenceType == DifferenceType.Added)
            {
                _addedLines.Add(line);
            }
        }

        private void SetToIndex(int to)
        {
            _to = to;
        }

        public bool Equals(DifferenceRange other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _differenceType == other._differenceType && _from == other._from && _to == other._to;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DifferenceRange)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)_differenceType;
                hashCode = (hashCode * 397) ^ _from;
                hashCode = (hashCode * 397) ^ _to;
                return hashCode;
            }
        }

        public static bool operator ==(DifferenceRange left, DifferenceRange right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DifferenceRange left, DifferenceRange right)
        {
            return !Equals(left, right);
        }
    }
}