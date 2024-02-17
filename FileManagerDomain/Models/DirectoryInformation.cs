using System;

namespace FileManagerDomain.Models
{
    public class DirectoryInformation
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Parent { get; set; }
        public string Root { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}