using System;

namespace Merge
{
    public static class Merge
    {
        public static string Execute(string[] originalLines, string[] file1Lines, string[] file2Lines)
        {
            if (originalLines == null) 
                throw new ArgumentNullException("originalLines");
            if (file1Lines == null) 
                throw new ArgumentNullException("file1Lines");
            if (file2Lines == null) 
                throw new ArgumentNullException("file2Lines");

            throw new NotImplementedException();
        }
    }
}