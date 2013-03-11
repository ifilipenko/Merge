using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;

namespace Merge
{
    public class Algorithms
    {
        public class LCSMatrix<T>
        {
            private readonly int[,] _data;

            public LCSMatrix(int sequence1Length, int sequence2Length)
            {
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
        /// <returns></returns>
        public LCSMatrix<T> LargestCommonLengthMatrix<T>(T[] sequence1, T[] sequence2)
        {
            var matrix = new LCSMatrix<T>(sequence1.Length, sequence2.Length);
            if (matrix.IsEmpty)
                return matrix;

            for (int i = 0; i <= sequence1.Length; i++)
            {
                for (int j = 0; j <= sequence1.Length; j++)
                {
                    if (j == 0 || i == 0)
                    {
                        matrix[i, j] = 0;
                    }
                    else if (Equals(sequence1[i - 1], sequence2[j - 1]))
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
        /// <returns></returns>
        public T[] LargestCommonSubsequence<T>(T[] sequence1, T[] sequence2)
        {
            var matrix = LargestCommonLengthMatrix(sequence1, sequence2);
            return LargestCommonSubsequenceCore(matrix, sequence1.Length, sequence2.Length, sequence1, sequence2).ToArray();
        }

        private IEnumerable<T> LargestCommonSubsequenceCore<T>(LCSMatrix<T> matrix, int i, int j, T[] sequence1, T[] sequence2)
        {
            if (i == 0 || j == 0)
                return Enumerable.Empty<T>();

            if (Equals(sequence1[i - 1], sequence2[j - 1]))
            {
                var subsequence = LargestCommonSubsequenceCore(matrix, i - 1, j - 1, sequence1, sequence2);
                return subsequence.Concat(sequence1[i - 1].ToEnumerable());
            }

            return matrix[i - 1, j] > matrix[i, j - 1]
                       ? LargestCommonSubsequenceCore(matrix, i - 1, j, sequence1, sequence2)
                       : LargestCommonSubsequenceCore(matrix, i, j - 1, sequence1, sequence2);
        }
    }
}