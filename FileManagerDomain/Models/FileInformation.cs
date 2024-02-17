using System;

namespace FileManagerDomain.Models
{
    public class FileInformation
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Extension { get; set; }
        public DateTime ModifiedDate { get; set; }
        public long Size { get; set; }
    }
}