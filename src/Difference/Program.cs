using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Merge
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parameters = ProcessArguments(args);
                if (!parameters.IsInitialized)
                {
                    return;
                }

                var file1Lines  = File.ReadAllLines(parameters.FilePath1);
                var file2Lines  = File.ReadAllLines(parameters.FilePath2);
                var differences = Diff.GetLinesDifference(file1Lines, file2Lines);

                Print(differences);
                if (parameters.Merge)
                {
                    var merge = new Merge();
                    var mergedFileText = merge.MergeDifferences(differences);
                    Console.Write(mergedFileText);
                }
            }
            catch (Exception ex)
            {
                PrintException(ex);
            }
        }

        private static void Print(IEnumerable<Difference> differences)
        {
            foreach (var difference in differences)
            {
                Console.WriteLine(difference);
            }
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
                    parameters.SetFiles(args.Skip(1).Take(3).ToArray());
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
            Console.WriteLine("-diff  \t \"changed file path\" \"original file path\" \t\t\t get two files difference");
            Console.WriteLine("-merge \t \"file1 path\"        \"file2 path\"         \"original file path\" \t merge changes of two file");
        }

        private static void PrintException(Exception exception)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
            }
            finally
            {
                Console.ResetColor();
            }

            Console.WriteLine("Use parameter key -h, -help or ? for help.");
        }
    }
}