using BlazorFileManager.Models;

namespace BlazorFileManager.Services
{
    public interface IFileManagerClient
    {
        CurrentFolderViewModel CurrentFolder { get; set; }
        string ParentFolderDisplayName { get; set; }

        void AddAllDrivesToDisplayModel();

        void CreateNewFolder(string newDirectoryName);

        void DeleteFolder(string path, bool recursiveDeletion);

        void NavigateToSubFolder(string path);

        string GetImageDataForPreview(string path);

        public string GetTextFileContentsForPreview(string path);
    }
}