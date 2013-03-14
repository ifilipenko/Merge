using System;
using System.Collections.Generic;
using System.Linq;

namespace Merge
{
    public class SplittingRange
    {
        public DifferenceRange Before { get; set; }
        public DifferenceRange CuttedRange { get; set; }
        public DifferenceRange After { get; set; }

        public int BeforeLength
        {
            get { return Before == null ? 0 : Before.Length; }
        }

        public int AfterLength
        {
            get { return After == null ? 0 : After.Length; }
        }

        public int CuttedLength
        {
            get { return CuttedRange == null ? 0 : CuttedRange.Length; }
        }
    }

    public class DifferenceRange : IEquatable<DifferenceRange>
    {
        public static DifferenceRange NewLinesRange(IList<Line> lines)
        {
            var from = lines.First().Index;
            var to = lines.Last().Index;
            var range = new DifferenceRange(DifferenceType.Add, @from, to);
            range._addedLines.AddRange(lines);
            return range;
        }

        public static DifferenceRange DeletedLinesRange(int from, int to)
        {
            return new DifferenceRange(DifferenceType.Delete, @from, to);
        }

        private DifferenceType _differenceType;
        private int _from;
        private int _to;
        private DifferenceRange _conflictedWith;
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
            get
            {
                var length = _to - _from + 1;
                return length < 0 ? 0 : length;
            }
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

        public SplittingRange CutRange(int @from, int to)
        {
            var splittingRange = new SplittingRange();
            if (From < @from)
            {
                var before = new DifferenceRange(DifferenceType, From, @from - 1);
                if (_addedLines.Any())
                {
                    var addedLines = _addedLines.Take(before.Length).ToArray();
                    before._addedLines.AddRange(addedLines);
                }
                splittingRange.Before = before;
            }

            var cuttedTo = Math.Min(To, to);
            var cuttedRange = new DifferenceRange(DifferenceType, @from, cuttedTo);
            if (_addedLines.Any())
            {
                var addedLines = _addedLines.Skip(splittingRange.BeforeLength).Take(cuttedRange.Length).ToArray();
                cuttedRange._addedLines.AddRange(addedLines);
            }
            splittingRange.CuttedRange = cuttedRange;

            if (To > to)
            {
                var after = new DifferenceRange(DifferenceType, @from + 1, To);
                if (_addedLines.Any())
                {
                    var addedLines = _addedLines.Skip(splittingRange.BeforeLength + splittingRange.CuttedLength)
                                                .Take(after.Length)
                                                .ToArray();
                    after._addedLines.AddRange(addedLines);
                }
                splittingRange.After = after;
            }

            return splittingRange;
        }

        public DifferenceRange CutRangeFrom(int @from, DifferenceRange conflictedRange = null)
        {
            if (@from < From || @from > To)
                throw new ArgumentException("Index not contain in range bounds", "from");

            var differenceRange = new DifferenceRange(DifferenceType, @from, To, conflictedRange);
            if (DifferenceType != DifferenceType.Delete)
            {
                var addedLines = AddedLines.Skip(@from - From).ToArray();
                differenceRange._addedLines.AddRange(addedLines);
            }

            _to = _to - differenceRange.Length;
            return differenceRange;
        }

        public DifferenceRange CutRangeTo(int to, DifferenceRange conflictedRange = null)
        {
            if (to < From || to > To)
                throw new ArgumentException("Index not contain in range bounds", "to");

            var cutRange = new DifferenceRange(DifferenceType, From, to, conflictedRange);
            if (DifferenceType != DifferenceType.Delete)
            {
                var addedLines = _addedLines.Take(cutRange.Length).ToArray();
                cutRange._addedLines.AddRange(addedLines);
                _addedLines.RemoveRange(0, cutRange.Length);
            }

            _from = to + 1;
            return cutRange;
        }

        public void MarkReplace()
        {
            _differenceType = DifferenceType.Replace;
        }

        public DifferenceRange Clone()
        {
            return new DifferenceRange(this);
        }

        public void MakeConflictedWith(DifferenceRange range)
        {
            if (range == null)
                throw new ArgumentNullException("range");
            if (ReferenceEquals(range, this))
                throw new ArgumentException("Range can not conflict with itself", "range");
            if (HasConflict)
                throw new InvalidOperationException("Range is already has conflict");

            _conflictedWith = range;
        }

        private void ProcessLine(Line line)
        {
            if (DifferenceType != DifferenceType.Delete)
            {
                _addedLines.Add(line);
            }
        }

        private void SetToIndex(int to)
        {
            // todo: validate that is not less then From
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

        public override string ToString()
        {
            return string.Format("{0}: {1} - {2} ({3}), Has conflict: {4}", DifferenceType, From, To, Length, HasConflict);
        }
    }
}