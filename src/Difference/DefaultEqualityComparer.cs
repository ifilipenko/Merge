using System.Collections.Generic;

namespace Merge
{
    public class DefaultEqualityComparer<T> : EqualityComparer<T>
    {
        public override bool Equals(T x, T y)
        {
            if (!ReferenceEquals(x, null))
            {
                return !ReferenceEquals(y, null) && x.Equals(y);
            }
            return ReferenceEquals(y, null);
        }

        public override int GetHashCode(T obj)
        {
            return ReferenceEquals(obj, null) ? 0 : obj.GetHashCode();
        }
    }
}