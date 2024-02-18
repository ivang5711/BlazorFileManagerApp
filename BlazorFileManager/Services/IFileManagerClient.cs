using BlazorFileManager.Models;

namespace BlazorFileManager.Services
{
    public interface IFileManagerClient
    {
        CurrentFolderViewModel CurrentFolder { get; set; }
        string ParentFolderDisplayName { get; set; }
        string[] ImageTypesForPreview { get; set; }
        string[] TextTypesForPreview { get; set; }

        void AddAllDrivesToDisplayModel();

        void CreateNewFolder(string newDirectoryName);

        void DeleteFolder(string path, bool recursiveDeletion);

        void NavigateToSubFolder(string path);

        string GetImageDataForPreview(string path);

        string GetTextFileContentsForPreview(string path);
    }
}