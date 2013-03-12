using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Merge
{
    class Parameters
    {
        public string FilePath1 { get; set; }
        public string FilePath2 { get; set; }

        public bool Print { get; set; }

        public bool Merge { get; set; }

        public bool DiffOnly { get; set; }

        public bool IsInitalized
        {
            get { throw new NotImplementedException(); }
        }

        public void SetFiles(string[] files)
        {
            throw new NotImplementedException();
        }
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
                if (!parameters.IsInitalized)
                {
                    return;
                }

                var file1Lines  = File.ReadAllLines(parameters.FilePath1);
                var file2Lines  = File.ReadAllLines(parameters.FilePath2);
                var differences = Diff.GetLinesDifference(file1Lines, file2Lines);

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
                PrintException(ex);
            }
        }

        private static void Print(Difference[] differences)
        {
            throw new NotImplementedException();
        }

        private static Parameters ProcessArguments(string[] args)
        {
            var parameters = new Parameters();

            if (args == null || args.Length == 0)
                throw new ArgumentException("Arguments can not be null");
            switch (args[0].ToLower())
            {
                case "-diff":
                    if (args.Length < 3)
                        throw new ArgumentException("For diff requires 2 parameters to specify the files");
                    parameters.DiffOnly = true;
                    parameters.SetFiles(args.Skip(1).Take(2).ToArray());
                    break;
                case "-merge":
                    if (args.Length < 4)
                        throw new ArgumentException("For merge requires 3 parameters to specify the files");
                    parameters.Merge = true;
                    parameters.SetFiles(args.Skip(1).Take(2).ToArray());
                    break;
                case "?":
                case "-h":
                case "-help":
                    PrintHelp();
                    break;
            }

            return parameters;
        }

        private static void PrintHelp()
        {
            throw new NotImplementedException();
        }

        private static void PrintException(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}