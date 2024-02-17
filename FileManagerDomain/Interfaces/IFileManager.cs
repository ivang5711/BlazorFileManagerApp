using FileManagerDomain.Models;
using System.Collections.Generic;

namespace FileManagerDomain.Interfaces
{
    public interface IFileManager
    {
        void CreateFolder(string path);

        void DeleteFolder(string path, bool deleteWithContents);

        IEnumerable<DriveInformation> GetAllDrives();

        IEnumerable<DirectoryInformation> GetAllInnerDerictoriesInformation(string path);

        IEnumerable<FileInformation> GetAllInnerFilesInfo(string path);

        DirectoryInformation GetDirectoryInformation(string path);
    }
}