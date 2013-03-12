using System;

namespace Merge
{
    class Parameters
    {
        public string FilePath1 { get; set; }
        public string FilePath2 { get; set; }
        public string OriginalFilePath { get; set; }
        public bool Merge { get; set; }
        public bool DiffOnly { get; set; }

        public bool IsInitialized
        {
            get { return !string.IsNullOrWhiteSpace(FilePath1) && !string.IsNullOrWhiteSpace(OriginalFilePath); }
        }

        public void SetFiles(string[] files)
        {
            switch (files.Length)
            {
                case 2:
                    OriginalFilePath = files[0];
                    FilePath1 = files[1];
                    break;
                case 3:
                    OriginalFilePath = files[0];
                    FilePath1 = files[1];
                    FilePath2 = files[2];
                    break;
                default:
                    throw new ArgumentException("Unexpected files list length", "files");
            }
        }
    }
}