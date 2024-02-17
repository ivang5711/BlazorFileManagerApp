using FileManagerDomain.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileManagerApplication.Services
{
    public class FileManager
    {
        public IEnumerable<DriveInformation> GetAllDrives()
        {
            List<DriveInformation> result = new List<DriveInformation>();
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                result.Add(new DriveInformation
                {
                    Name = drive.Name,
                    Size = drive.TotalSize
                });
            }

            return result;
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

        //public DirectoryInfo GetDirectoryInfo(string path)
        //{
        //    return new DirectoryInfo(path);
        //}

        public DirectoryInformation GetDirectoryInfo(string path)
        {
            var directory = new DirectoryInfo(path);
            DirectoryInformation result = new DirectoryInformation
            {
                Name = directory.Name,
                FullName = directory.FullName,
                Parent = directory.Parent?.FullName ?? string.Empty,
                Root = directory.Root.FullName,
                ModifiedDate = directory.LastWriteTime
            };

            return result;
        }

        public IEnumerable<DirectoryInformation> GetAllInnerDerictoriesInfo(string directoryPath)
        {
            DirectoryInfo[] directories = new DirectoryInfo(directoryPath).GetDirectories();
            List<DirectoryInformation> result = new List<DirectoryInformation>();
            foreach (var d in directories)
            {
                result.Add(new DirectoryInformation()
                {
                    Name = d.Name,
                    FullName = d.FullName,
                    Parent = d.Parent.FullName,
                    Root = d.Root.FullName,
                    ModifiedDate = d.LastWriteTime,
                });
            }

            return result;
        }

        public IEnumerable<FileInformation> GetAllInnerFilesInfo(string directoryPath)
        {
            FileInfo[] directories = new DirectoryInfo(directoryPath).GetFiles();
            List<FileInformation> result = new List<FileInformation>();
            foreach (var d in directories)
            {
                result.Add(new FileInformation()
                {
                    Name = d.Name,
                    FullName = d.FullName,
                    Extension = d.Extension,
                    ModifiedDate = d.LastWriteTime,
                    Size = d.Length,
                });
            }

            return result;
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "An error occured while deleting directory";
            }

            return string.Empty;
        }
    }
}