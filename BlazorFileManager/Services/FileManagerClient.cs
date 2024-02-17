using BlazorFileManager.Models;
using FileManagerDomain.Exceptions;
using FileManagerDomain.Interfaces;
using FileManagerDomain.Models;
using Humanizer;

namespace BlazorFileManager.Services;

public class FileManagerClient : IFileManagerClient
{
    private readonly IFileManager _fileManager;
    public string ParentFolderDisplayName { get; set; } = "..";
    public CurrentFolderViewModel CurrentFolder { get; set; } = new();

    public FileManagerClient(IFileManager fileManager)
    {
        _fileManager = fileManager;
    }

    public void CreateNewFolder(string newDirectoryName)
    {
        string newFolder = Path
            .Combine(CurrentFolder!.FullPath, newDirectoryName);
        CreateFolder(newFolder);
        NavigateToSubFolder(CurrentFolder.FullPath);
    }

    private void CreateFolder(string newFolder)
    {
        try
        {
            _fileManager.CreateFolder(newFolder);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public void DeleteFolder(string path, bool recursiveDeletion)
    {
        try
        {
            _fileManager.DeleteFolder(path,
                recursiveDeletion);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    public void NavigateToSubFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            CurrentFolder = new();
            AddAllDrivesToDisplayModel();
            return;
        }

        InitializeDirectory(path);
    }

    private void InitializeDirectory(string path)
    {
        DirectoryInformation directoryInformation =
            GetDirectoryInformation(path);
        string parentDirectory = GetParentDirectory(path, directoryInformation);
        IEnumerable<DirectoryInformation> directories = GetDirectories(path);
        IEnumerable<FileInformation> files = GetFiles(path);
        InitializeCurrentFolder(path, parentDirectory, directories, files);
    }

    private void InitializeCurrentFolder(string path, string temp,
        IEnumerable<DirectoryInformation> directories,
        IEnumerable<FileInformation> files)
    {
        CurrentFolder = new CurrentFolderViewModel()
        {
            Name = path,
            FullPath = path,
            Parent = temp,
            IsRoot = false,
        };
        CurrentFolder.InnerItems.Add(CreateParentItem(temp));
        AddAllFoldersToDisplayModel(directories);
        AddAllFilesToDisplayModel(files);
    }

    private string GetParentDirectory(string path, DirectoryInformation directoryInformation)
    {
        string temp = directoryInformation.Parent;
        if (CurrentFolder!.InnerItems[0].FullName == path)
        {
            temp ??= string.Empty;
        }
        else
        {
            temp ??= directoryInformation.Root;
        }

        return temp;
    }

    private IEnumerable<FileInformation> GetFiles(string path)
    {
        IEnumerable<FileInformation> files;
        try
        {
            files = _fileManager.GetAllInnerFilesInfo(path);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        return files;
    }

    private IEnumerable<DirectoryInformation> GetDirectories(string path)
    {
        IEnumerable<DirectoryInformation> directories;
        try
        {
            directories = _fileManager
                .GetAllInnerDerictoriesInformation(path);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        return directories;
    }

    private DirectoryInformation GetDirectoryInformation(string path)
    {
        DirectoryInformation directoryInformation;
        try
        {
            directoryInformation = _fileManager.GetDirectoryInformation(path);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        return directoryInformation;
    }

    public void AddAllDrivesToDisplayModel()
    {
        IEnumerable<DriveInformation> drives = GetDrives();
        List<FileSystemItemViewModel> drivesViewModels = GetDrivesViewModels(drives);

        CurrentFolder = new CurrentFolderViewModel()
        {
            IsRoot = true,
        };
        CurrentFolder.InnerItems.AddRange(drivesViewModels);
    }

    private static List<FileSystemItemViewModel> GetDrivesViewModels(IEnumerable<DriveInformation> drives)
    {
        List<FileSystemItemViewModel> drivesViewModels = new();
        foreach (var drive in drives)
        {
            drivesViewModels.Add(new FileSystemItemViewModel()
            {
                Name = drive.Name,
                FullName = drive.Name,
                Parent = string.Empty,
                Extension = string.Empty,
                Size = drive.Size.Bytes().Humanize(),
            });
        }

        return drivesViewModels;
    }

    private IEnumerable<DriveInformation> GetDrives()
    {
        IEnumerable<DriveInformation> drives;
        try
        {
            drives = _fileManager.GetAllDrives();
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        return drives;
    }

    private void AddAllFoldersToDisplayModel(
        IEnumerable<DirectoryInformation> folders)
    {
        List<FileSystemItemViewModel> foldersViewModels =
            GetFoldersViewModels(folders);

        CurrentFolder!.InnerItems.AddRange(foldersViewModels);
    }

    private static List<FileSystemItemViewModel> GetFoldersViewModels(
        IEnumerable<DirectoryInformation> folders)
    {
        List<FileSystemItemViewModel> foldersViewModels = new();
        foreach (var folder in folders)
        {
            foldersViewModels.Add(new FileSystemItemViewModel()
            {
                Name = folder.Name,
                FullName = folder.FullName,
                Parent = folder.Parent,
                Extension = string.Empty,
                ModifiedDate = folder.ModifiedDate.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")
            });
        }

        return foldersViewModels;
    }

    private void AddAllFilesToDisplayModel(
        IEnumerable<FileInformation> files)
    {
        List<FileSystemItemViewModel> filesViewModels = GetFilesViewModels(files);

        CurrentFolder!.InnerItems.AddRange(filesViewModels);
    }

    private static List<FileSystemItemViewModel> GetFilesViewModels(IEnumerable<FileInformation> files)
    {
        List<FileSystemItemViewModel> filesViewModels = new();
        foreach (var file in files)
        {
            filesViewModels.Add(new FileSystemItemViewModel()
            {
                Name = file.Name,
                FullName = file.FullName,
                Parent = string.Empty,
                Extension = file.Extension,
                Size = file.Size.Bytes().Humanize(),
                ModifiedDate = file.ModifiedDate.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")
            });
        }

        return filesViewModels;
    }

    private FileSystemItemViewModel CreateParentItem(string parent)
    {
        return new FileSystemItemViewModel()
        {
            Name = ParentFolderDisplayName,
            FullName = parent,
            Parent = parent,
            Extension = string.Empty,
            Size = string.Empty,
        };
    }
}