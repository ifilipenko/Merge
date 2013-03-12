using System;

namespace Merge
{
    class Parameters
    {
        public string FilePath1 { get; set; }
        public string FilePath2 { get; set; }
        public string FilePath3 { get; set; }
        public bool Merge { get; set; }
        public bool DiffOnly { get; set; }

        public bool IsInitialized
        {
            get { return !string.IsNullOrWhiteSpace(FilePath1) && string.IsNullOrWhiteSpace(FilePath2); }
        }

        public void SetFiles(string[] files)
        {
            switch (files.Length)
            {
                case 2:
                    FilePath1 = files[0];
                    FilePath2 = files[1];
                    break;
                case 3:
                    FilePath1 = files[0];
                    FilePath2 = files[1];
                    FilePath3 = files[2];
                    break;
                default:
                    throw new ArgumentException("Unexpected files list length", "files");
            }
        }
    }
}