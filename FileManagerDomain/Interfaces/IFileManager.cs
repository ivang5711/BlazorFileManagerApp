using FileManagerDomain.Models;
using System.Collections.Generic;

namespace FileManagerDomain.Interfaces
{
    public interface IFileManager
    {
        string CreateFolder(string path);   
        string DeleteFolder(string path, bool deleteWithContents);
        IEnumerable<DriveInformation> GetAllDrives();
        IEnumerable<DirectoryInformation> GetAllInnerDerictoriesInfo(string Path);
        IEnumerable<FileInformation> GetAllInnerFilesInfo(string Path);
        DirectoryInformation GetDirectoryInfo(string path);
    }
}