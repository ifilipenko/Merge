using System;
using System.IO;

namespace Merge
{
    class Parameters
    {
        public string FilePath1 { get; set; }
        public string FilePath2 { get; set; }

        public bool Print { get; set; }

        public bool Merge { get; set; }
    }

    class Merge
    {
        public string MergeDifferences(Difference[] differences)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parameters = ProcessArguments(args);

                var diff = new Diff();
                var file1Lines = File.ReadAllLines(parameters.FilePath1);
                var file2Lines = File.ReadAllLines(parameters.FilePath2);
                var differences = diff.GetLinesDifference(file1Lines, file2Lines);

                if (parameters.Print)
                {
                    Print(differences);
                }
                if (parameters.Merge)
                {
                    var merge = new Merge();
                    var mergedFileText = merge.MergeDifferences(differences);
                    Console.Write(mergedFileText);
                }
            }
            catch (ApplicationException ex)
            {
                Console.Write(ex.Message);
                PrintHelp();
            }
        }

        private static void Print(Difference[] differences)
        {
            throw new NotImplementedException();
        }

        private static Parameters ProcessArguments(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ApplicationException("Required two file pathes");
            }

            return new Parameters();
        }

        private static void PrintHelp()
        {
            throw new NotImplementedException();
        }
    }
}