﻿namespace BlazorFileManager.Services
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
            List<FileInfo> result = new();
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
    }
}