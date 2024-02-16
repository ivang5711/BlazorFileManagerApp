using System;
using System.Collections.Generic;
using System.IO;

namespace FileManagerApplication.Services
{
    public class FileManager
    {
        public IEnumerable<DriveInfo> GetAllDrives()
        {
            return DriveInfo.GetDrives();
        }

        public IEnumerable<string> GetAllDirectoryFoldersFullNames(string path)
        {
            return Directory
                .EnumerateDirectories(Directory.GetDirectoryRoot(path));
        }

        public IEnumerable<string> GetAllDirectoryFilestFullFileNames(string path)
        {
            return Directory.GetFiles(Directory.GetDirectoryRoot(path),
                "*.*", SearchOption.TopDirectoryOnly);
        }

        public List<FileInfo> GetAllDirectoryFilesInfo(IEnumerable<string> fileNames)
        {
            List<FileInfo> result = new List<FileInfo>();
            foreach (string fileName in fileNames)
            {
                result.Add(new FileInfo(fileName));
            }

            return result;
        }

        public DirectoryInfo GetDirectoryInfo(string path)
        {
            return new DirectoryInfo(path);
        }

        public string CreateFolder(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "An error occured while creating directory";
            }

            return string.Empty;
        }

        public string DeleteFolder(string path, bool deleteWithContents)
        {
            try
            {
                Directory.Delete(path, deleteWithContents);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return "An error occured while deleting directory";
            }

            return string.Empty;
        }
    }
}