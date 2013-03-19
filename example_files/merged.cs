Merge
	Algorithms_original.cs
With files
	Algorithms_changed.cs
	Algorithms_changed2.cs
using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;

namespace Merge
{
    public static class Algorithms
    {
        public class Matrix<T>
        {
            private readonly int[,] _data;

            public Matrix(int sequence1Length, int sequence2Length)
            {
<<<
                _data = new int[sequence1Length + 1, sequence2Length + 1];
---
                _data = new int[ sequence1Length, sequence2Length ];
>>>
                _data = new int[sequence1Length, sequence2Length];
            }

            public int MaxLength { get; private set; }

            public bool IsEmpty
            {
                get { return Sequence1Length == 0 || Sequence2Length == 0; }
            }

            public int Sequence1Length
            {
                get { return _data.GetLength(0); }
            }

            public int Sequence2Length
            {
                get { return _data.GetLength(0); }
            }

            public int this[int i, int j]
            {
                get { return _data[i, j]; }
                set
                {
                    _data[i, j] = value;
                    if (MaxLength < value)
                    {
                        MaxLength = value;
                    }
                }
            }
        }

        /// <summary>
        /// Implementation algorithm described in
        /// http://www.algorithmist.com/index.php/Longest_Common_Subsequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence1"></param>
        /// <param name="sequence2"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static Matrix<T> LargestCommonLengthMatrix<T>(T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer = null)
        {
            var matrix = new Matrix<T>(sequence1.Length, sequence2.Length);
            if (matrix.IsEmpty)
                return matrix;

            if (equalityComparer == null)
            {
                equalityComparer = new DefaultEqualityComparer<T>();
            }

            for (int i = 0; i <= sequence1.Length; i++)
            {
                for (int j = 0; j <= sequence1.Length; j++)
                {
                    if (j == 0 || i == 0)
                    {
                        matrix[i, j] = 0;
                    }
                    else if (equalityComparer.Equals(sequence1[i - 1], sequence2[j - 1]))
                    {
                        matrix[i, j] = matrix[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        matrix[i, j] = Math.Max(matrix[i - 1, j], matrix[i, j - 1]);
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// Implementation algorithm described in
        /// http://www.algorithmist.com/index.php/Longest_Common_Subsequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence1"></param>
        /// <param name="sequence2"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static T[] LargestCommonSubsequence<T>(T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer = null)
        {
            var matrix = LargestCommonLengthMatrix(sequence1, sequence2);
            return LargestCommonSubsequenceCore(matrix, sequence1.Length, sequence2.Length, sequence1, sequence2, equalityComparer).ToArray();
        }

        private static IEnumerable<T> LargestCommonSubsequenceCore<T>(Matrix<T> matrix, int i, int j, T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer)
        {
            if (i == 0 || j == 0)
                return Enumerable.Empty<T>();

            if (equalityComparer.Equals(sequence1[i - 1], sequence2[j - 1]))
            {
                var subsequence = LargestCommonSubsequenceCore(matrix, i - 1, j - 1, sequence1, sequence2, equalityComparer);
                return subsequence.Concat(sequence1[i - 1].ToEnumerable());
            }

            return matrix[i - 1, j] > matrix[i, j - 1]
                       ? LargestCommonSubsequenceCore(matrix, i - 1, j, sequence1, sequence2, equalityComparer)
                       : LargestCommonSubsequenceCore(matrix, i, j - 1, sequence1, sequence2, equalityComparer);
        }
    }
}Merge
	Algorithms_original.cs
With files
	Algorithms_changed.cs
	Algorithms_changed2.cs
using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;

namespace Merge
{
    public static class Algorithms
    {
        public class Matrix<T>
        {
            private readonly int[,] _data;

            public Matrix(int sequence1Length, int sequence2Length)
            {
<<<
                _data = new int[sequence1Length + 1, sequence2Length + 1];
---
                _data = new int[ sequence1Length, sequence2Length ];
>>>
                _data = new int[sequence1Length, sequence2Length];
            }

            public int MaxLength { get; private set; }

            public bool IsEmpty
            {
                get { return Sequence1Length == 0 || Sequence2Length == 0; }
            }

            public int Sequence1Length
            {
                get { return _data.GetLength(0); }
            }

            public int Sequence2Length
            {
                get { return _data.GetLength(0); }
            }

            public int this[int i, int j]
            {
                get { return _data[i, j]; }
                set
                {
                    _data[i, j] = value;
                    if (MaxLength < value)
                    {
                        MaxLength = value;
                    }
                }
            }
        }

        /// <summary>
        /// Implementation algorithm described in
        /// http://www.algorithmist.com/index.php/Longest_Common_Subsequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence1"></param>
        /// <param name="sequence2"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static Matrix<T> LargestCommonLengthMatrix<T>(T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer = null)
        {
            var matrix = new Matrix<T>(sequence1.Length, sequence2.Length);
            if (matrix.IsEmpty)
                return matrix;

            if (equalityComparer == null)
            {
                equalityComparer = new DefaultEqualityComparer<T>();
            }

            for (int i = 0; i <= sequence1.Length; i++)
            {
                for (int j = 0; j <= sequence1.Length; j++)
                {
                    if (j == 0 || i == 0)
                    {
                        matrix[i, j] = 0;
                    }
                    else if (equalityComparer.Equals(sequence1[i - 1], sequence2[j - 1]))
                    {
                        matrix[i, j] = matrix[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        matrix[i, j] = Math.Max(matrix[i - 1, j], matrix[i, j - 1]);
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// Implementation algorithm described in
        /// http://www.algorithmist.com/index.php/Longest_Common_Subsequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence1"></param>
        /// <param name="sequence2"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static T[] LargestCommonSubsequence<T>(T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer = null)
        {
            var matrix = LargestCommonLengthMatrix(sequence1, sequence2);
            return LargestCommonSubsequenceCore(matrix, sequence1.Length, sequence2.Length, sequence1, sequence2, equalityComparer).ToArray();
        }

        private static IEnumerable<T> LargestCommonSubsequenceCore<T>(Matrix<T> matrix, int i, int j, T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer)
        {
            if (i == 0 || j == 0)
                return Enumerable.Empty<T>();

            if (equalityComparer.Equals(sequence1[i - 1], sequence2[j - 1]))
            {
                var subsequence = LargestCommonSubsequenceCore(matrix, i - 1, j - 1, sequence1, sequence2, equalityComparer);
                return subsequence.Concat(sequence1[i - 1].ToEnumerable());
            }

            return matrix[i - 1, j] > matrix[i, j - 1]
                       ? LargestCommonSubsequenceCore(matrix, i - 1, j, sequence1, sequence2, equalityComparer)
                       : LargestCommonSubsequenceCore(matrix, i, j - 1, sequence1, sequence2, equalityComparer);
        }
    }
}Merge
	Algorithms_original.cs
With files
	Algorithms_changed.cs
	Algorithms_changed2.cs
using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;

namespace Merge
{
    public static class Algorithms
    {
        public class Matrix<T>
        {
            private readonly int[,] _data;

            public Matrix(int sequence1Length, int sequence2Length)
            {
<<<
+                _data = new int[sequence1Length + 1, sequence2Length + 1];
---
+                _data = new int[ sequence1Length, sequence2Length ];
>>>
                _data = new int[sequence1Length, sequence2Length];
            }

            public int MaxLength { get; private set; }

            public bool IsEmpty
            {
                get { return Sequence1Length == 0 || Sequence2Length == 0; }
            }

            public int Sequence1Length
            {
                get { return _data.GetLength(0); }
            }

            public int Sequence2Length
            {
                get { return _data.GetLength(0); }
            }

            public int this[int i, int j]
            {
                get { return _data[i, j]; }
                set
                {
                    _data[i, j] = value;
                    if (MaxLength < value)
                    {
                        MaxLength = value;
                    }
                }
            }
        }

        /// <summary>
        /// Implementation algorithm described in
        /// http://www.algorithmist.com/index.php/Longest_Common_Subsequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence1"></param>
        /// <param name="sequence2"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static Matrix<T> LargestCommonLengthMatrix<T>(T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer = null)
        {
            var matrix = new Matrix<T>(sequence1.Length, sequence2.Length);
            if (matrix.IsEmpty)
                return matrix;

            if (equalityComparer == null)
            {
                equalityComparer = new DefaultEqualityComparer<T>();
            }

            for (int i = 0; i <= sequence1.Length; i++)
            {
                for (int j = 0; j <= sequence1.Length; j++)
                {
                    if (j == 0 || i == 0)
                    {
                        matrix[i, j] = 0;
                    }
                    else if (equalityComparer.Equals(sequence1[i - 1], sequence2[j - 1]))
                    {
                        matrix[i, j] = matrix[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        matrix[i, j] = Math.Max(matrix[i - 1, j], matrix[i, j - 1]);
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// Implementation algorithm described in
        /// http://www.algorithmist.com/index.php/Longest_Common_Subsequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence1"></param>
        /// <param name="sequence2"></param>
        /// <param name="equalityComparer"></param>
        /// <returns></returns>
        public static T[] LargestCommonSubsequence<T>(T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer = null)
        {
            var matrix = LargestCommonLengthMatrix(sequence1, sequence2);
            return LargestCommonSubsequenceCore(matrix, sequence1.Length, sequence2.Length, sequence1, sequence2, equalityComparer).ToArray();
        }

        private static IEnumerable<T> LargestCommonSubsequenceCore<T>(Matrix<T> matrix, int i, int j, T[] sequence1, T[] sequence2, IEqualityComparer<T> equalityComparer)
        {
            if (i == 0 || j == 0)
                return Enumerable.Empty<T>();

            if (equalityComparer.Equals(sequence1[i - 1], sequence2[j - 1]))
            {
                var subsequence = LargestCommonSubsequenceCore(matrix, i - 1, j - 1, sequence1, sequence2, equalityComparer);
                return subsequence.Concat(sequence1[i - 1].ToEnumerable());
            }

            return matrix[i - 1, j] > matrix[i, j - 1]
                       ? LargestCommonSubsequenceCore(matrix, i - 1, j, sequence1, sequence2, equalityComparer)
                       : LargestCommonSubsequenceCore(matrix, i, j - 1, sequence1, sequence2, equalityComparer);
        }
    }
}