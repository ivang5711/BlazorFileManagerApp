using FileManagerDomain.Exceptions;
using FileManagerDomain.Interfaces;
using FileManagerDomain.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileManagerApplication.Services
{
    public class FileManager : IFileManager
    {
        public IEnumerable<DriveInformation> GetAllDrives()
        {
            DriveInfo[] drives;
            try
            {
                drives = DriveInfo.GetDrives();
            }
            catch (UnauthorizedAccessException ex)
            {
                string errorMessage = "You do not have permission to access this directory";
                throw new AccessDeniedException(errorMessage, ex);
            }
            catch (IOException ex)
            {
                string errorMessage = "Failed to access the requested directory";
                throw new InnerErrorException(errorMessage, ex);
            }

            return GetAllDrivesInformation(drives);
        }

        private static List<DriveInformation> GetAllDrivesInformation(DriveInfo[] drives)
        {
            List<DriveInformation> result = new List<DriveInformation>();
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

        public DirectoryInformation GetDirectoryInfo(string path)
        {
            return GetDirectoryInformation(new DirectoryInfo(path));
        }

        private static DirectoryInformation GetDirectoryInformation(DirectoryInfo directory)
        {
            return new DirectoryInformation
            {
                Name = directory.Name,
                FullName = directory.FullName,
                Parent = directory.Parent?.FullName ?? string.Empty,
                Root = directory.Root.FullName,
                ModifiedDate = directory.LastWriteTime
            };
        }

        public IEnumerable<DirectoryInformation> GetAllInnerDerictoriesInfo(string Path)
        {
            DirectoryInfo[] directories = new DirectoryInfo(Path).GetDirectories();
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

        public IEnumerable<FileInformation> GetAllInnerFilesInfo(string Path)
        {
            FileInfo[] directories = new DirectoryInfo(Path).GetFiles();
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