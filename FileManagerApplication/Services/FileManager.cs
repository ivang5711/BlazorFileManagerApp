using FileManagerDomain.Exceptions;
using FileManagerDomain.Interfaces;
using FileManagerDomain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

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
                string errorMessage = 
                    "You do not have permission to access this directory";
                throw new AccessDeniedException(errorMessage, ex);
            }
            catch (IOException ex)
            {
                string errorMessage = 
                    "Failed to access the requested directory";
                throw new InnerErrorException(errorMessage, ex);
            }

            return GetAllDrivesInformation(drives);
        }

        private static List<DriveInformation> GetAllDrivesInformation(
            DriveInfo[] drives)
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

        public DirectoryInformation GetDirectoryInformation(string path)
        {
            return GetDirectoryInformationByDirectoryInfo(
                GetDirectoryInfoByPath(path));
        }

        private static DirectoryInfo GetDirectoryInfoByPath(string path)
        {
            DirectoryInfo directoryInfo;
            try
            {
                directoryInfo = new DirectoryInfo(path);
            }
            catch (ArgumentNullException ex)
            {
                string errorMessage = "Path should not be null";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (ArgumentException ex)
            {
                string errorMessage = "Check path provided";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (PathTooLongException ex)
            {
                string errorMessage = "The path provided is too long";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (SecurityException ex)
            {
                string errorMessage = 
                    "Failed to get the requested directory info";
                throw new InnerErrorException(errorMessage, ex);
            }

            return directoryInfo;
        }

        private static DirectoryInformation 
            GetDirectoryInformationByDirectoryInfo(DirectoryInfo directory)
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

        public IEnumerable<DirectoryInformation>
            GetAllInnerDerictoriesInformation(string path)
        {
            DirectoryInfo[] directories = GetInnerDirectoriesByPath(path);
            return GetAllInnerDirectories(directories);
        }

        private static DirectoryInfo[] GetInnerDirectoriesByPath(string path)
        {
            DirectoryInfo[] directories;
            try
            {
                directories = GetDirectoryInfoByPath(path).GetDirectories();
            }
            catch (UnauthorizedAccessException ex)
            {
                string errorMessage = 
                    "You do not have permission to access this directory";
                throw new AccessDeniedException(errorMessage, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                string errorMessage = "The requested directory was not found";
                throw new InnerErrorException(errorMessage, ex);
            }
            catch (SecurityException ex)
            {
                string errorMessage = 
                    "Failed to get the requested directory info";
                throw new InnerErrorException(errorMessage, ex);
            }

            return directories;
        }

        private static List<DirectoryInformation> 
            GetAllInnerDirectories(DirectoryInfo[] directories)
        {
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

        public IEnumerable<FileInformation> GetAllInnerFilesInfo(string path)
        {
            FileInfo[] directories = GetInnerFilesInfo(path);
            return GetAllInnerFilesByPath(directories);
        }

        private static FileInfo[] GetInnerFilesInfo(string path)
        {
            FileInfo[] directories;
            try
            {
                directories = GetDirectoryInfoByPath(path).GetFiles();
            }
            catch (DirectoryNotFoundException ex)
            {
                string errorMessage = "The requested directory was not found";
                throw new InnerErrorException(errorMessage, ex);
            }

            return directories;
        }

        private static List<FileInformation> 
            GetAllInnerFilesByPath(FileInfo[] directories)
        {
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

        public void CreateFolder(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (UnauthorizedAccessException ex)
            {
                string errorMessage = 
                    "You do not have permission to access this directory";
                throw new AccessDeniedException(errorMessage, ex);
            }
            catch (ArgumentNullException ex)
            {
                string errorMessage = "Path should not be null";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (ArgumentException ex)
            {
                string errorMessage = "Check path provided";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (PathTooLongException ex)
            {
                string errorMessage = "The path provided is too long";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                string errorMessage = "Directory can not be found";
                throw new InnerErrorException(errorMessage, ex);
            }
            catch (NotSupportedException ex)
            {
                string errorMessage = "Failed to create new directory";
                throw new InnerErrorException(errorMessage, ex);
            }
            catch (IOException ex)
            {
                string errorMessage = "Failed to create new directory";
                throw new InnerErrorException(errorMessage, ex);
            }
        }

        public void DeleteFolder(string path, bool deleteWithContents)
        {
            try
            {
                Directory.Delete(path, deleteWithContents);
            }
            catch (UnauthorizedAccessException ex)
            {
                string errorMessage = 
                    "You do not have permission to access this directory";
                throw new AccessDeniedException(errorMessage, ex);
            }
            catch (ArgumentNullException ex)
            {
                string errorMessage = "Path should not be null";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (ArgumentException ex)
            {
                string errorMessage = "Check path provided";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (PathTooLongException ex)
            {
                string errorMessage = "The path provided is too long";
                throw new PathArgumentException(errorMessage, ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                string errorMessage = "Directory can not be found";
                throw new InnerErrorException(errorMessage, ex);
            }
            catch (IOException ex)
            {
                string errorMessage = "Failed to delete the directory";
                throw new InnerErrorException(errorMessage, ex);
            }
        }
    }
}