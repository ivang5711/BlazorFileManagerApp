using BlazorFileManager.Models;
using FileManagerDomain.Models;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;

namespace BlazorFileManager.Pages;

public partial class Index
{
    public string? NewDirectoryName { get; set; }
    private CurrentFolderViewModel? _currentFolder;
    private readonly string type = "Click";
    private bool isFolderSelected = false;
    private string? _errorMessage = string.Empty;

    private readonly IList<Tuple<FileSystemItemViewModel,
        RadzenDataGridColumn<FileSystemItemViewModel>>> selectedCellData =
            new List<Tuple<FileSystemItemViewModel,
                RadzenDataGridColumn<FileSystemItemViewModel>>>();

    private RadzenButton button;
    private RadzenButton button2;
    private Popup popup;
    private Popup popup2;
    private bool _deleteFolderWithContents;

    protected override async Task OnInitializedAsync()
    {
        var y = selectedCellData.Any();
        NewDirectoryName = null;
        _currentFolder = new();
        AddAllDrivesToDisplayModel();
    }

    private void CreateNewFolder(string newDirectoryName)
    {
        var newFolder = System.IO.Path
            .Combine(_currentFolder.FullPath, newDirectoryName);
        _fileManager.CreateFolder(newFolder);
        Console.WriteLine("New folder created!");
        GoToSubFolder(_currentFolder.FullPath);
    }

    private void DeleteFolder()
    {
        _errorMessage = _fileManager
            .DeleteFolder(selectedCellData.First().Item1.FullName, _deleteFolderWithContents);
    }

    private void Select(DataGridCellMouseEventArgs<FileSystemItemViewModel> args)
    {
        var cellData = selectedCellData.FirstOrDefault(
            i => i.Item1 == args.Data && i.Item2 == args.Column);
        if (cellData != null)
        {
            selectedCellData.RemoveAt(0);
            isFolderSelected = false;
        }
        else
        {
            selectedCellData.Add(
                new Tuple<FileSystemItemViewModel,
                RadzenDataGridColumn<FileSystemItemViewModel>>(
                    args.Data, args.Column));
            if (selectedCellData.Count > 1)
            {
                selectedCellData.RemoveAt(0);
            }

            isFolderSelected = true;
        }
    }

    private void OnCellClick(
        DataGridCellMouseEventArgs<FileSystemItemViewModel> args)
    {
        if (type == "Click")
        {
            Select(args);
            _errorMessage = null;
        }
    }

    private void OnCellDoubleClick(
        DataGridCellMouseEventArgs<FileSystemItemViewModel> args)
    {
        if (type != "Click")
        {
            Select(args);
        }

        _errorMessage = null;
        GoToSubFolder(args.Data.FullName);
    }

    private void GoToSubFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _currentFolder = new();
            AddAllDrivesToDisplayModel();
            return;
        }

        var dirInfo = _fileManager.GetDirectoryInfo(path);
        var temp = dirInfo.Parent;
        if (_currentFolder.InnerItems[0].FullName == path)
        {
            temp ??= string.Empty;
        }
        else
        {
            temp ??= dirInfo.Root;
        }

        var directories = _fileManager.GetAllInnerDerictoriesInfo(path);
        var files = _fileManager.GetAllInnerFilesInfo(path);
        //try
        //{
        //    directories = dirInfo.GetDirectories();
        //}
        //catch (UnauthorizedAccessException ex)
        //{
        //    _errorMessage = "You do not have permission to access this directory";
        //    Console.WriteLine(ex);
        //    return;
        //}
        //catch (DirectoryNotFoundException ex)
        //{
        //    _errorMessage = "Failed to access the requested directory";
        //    Console.WriteLine(ex);
        //    return;
        //}

        _currentFolder = new();
        _currentFolder = new CurrentFolderViewModel()
        {
            Name = path,
            FullPath = path,
            Parent = temp,
            IsRoot = false,
        };
        _currentFolder.InnerItems.Add(CreateParentItem(temp));
        AddAllFoldersToDisplayModel(directories);
        AddAllFilesToDisplayModel(files);
    }

    private void AddAllFoldersToDisplayModel(IEnumerable<DirectoryInformation> folders)
    {
        List<FileSystemItemViewModel> temp = new();
        foreach (var folder in folders)
        {
            temp.Add(new FileSystemItemViewModel()
            {
                Name = folder.Name,
                FullName = folder.FullName,
                Parent = folder.Parent,
                Extension = string.Empty,
                ModifiedDate = folder.ModifiedDate
            });
        }

        _currentFolder.InnerItems.AddRange(temp);
    }

    private void AddAllFilesToDisplayModel(IEnumerable<FileInformation> files)
    {
        List<FileSystemItemViewModel> temp = new();
        foreach (var file in files)
        {
            temp.Add(new FileSystemItemViewModel()
            {
                Name = file.Name,
                FullName = file.FullName,
                Parent = string.Empty,
                Extension = file.Extension,
                Size = file.Size,
                ModifiedDate = file.ModifiedDate
            });
        }

        _currentFolder.InnerItems.AddRange(temp);
    }

    private void AddAllDrivesToDisplayModel()
    {
        var drives = _fileManager.GetAllDrives();
        List<FileSystemItemViewModel> temp = new();
        foreach (var drive in drives)
        {
            temp.Add(new FileSystemItemViewModel()
            {
                Name = drive.Name,
                FullName = drive.Name,
                Parent = string.Empty,
                Extension = string.Empty,
                Size = drive.Size,
            });
        }

        _currentFolder = new CurrentFolderViewModel()
        {
            IsRoot = true,
        };
        _currentFolder.InnerItems.AddRange(temp);
    }

    private FileSystemItemViewModel CreateParentItem(string parent)
    {
        return new FileSystemItemViewModel()
        {
            Name = "..",
            FullName = parent,
            Parent = parent,
            Extension = string.Empty,
            Size = 0,
        };
    }

    private async Task OnOpen()
    {
        await JSRuntime.InvokeVoidAsync(
            "eval",
            "setTimeout(function(){ document.getElementById('search').focus(); }, 200)");
    }

    private async Task OnOpen2()
    {
        await JSRuntime.InvokeVoidAsync(
            "eval",
            "setTimeout(function(){ document.getElementById('search').focus(); }, 200)");
    }

    private async Task CloseCreateDirectoryDialog()
    {
        await popup.CloseAsync();
    }

    private async Task CloseDeleteDirectoryDialog()
    {
        await popup2.CloseAsync();
    }

    private void CreateNewDirectory()
    {
        CreateNewFolder(NewDirectoryName);
        NewDirectoryName = null;
        CloseCreateDirectoryDialog();
    }

    private void ConfirmDeleteDirectory()
    {
        DeleteFolder();
        CloseDeleteDirectoryDialog();
        GoToSubFolder(_currentFolder.FullPath);
    }

    private void OnCellRender(DataGridCellRenderEventArgs<FileSystemItemViewModel> args)
    {
        if (selectedCellData
            .Any(i => i.Item1 == args.Data && i.Item2 == args.Column))
        {
            args.Attributes.Add("style",
                $"background-color: var(--rz-secondary-lighter);");
        }
    }

    private void OnChange3(bool? value, string name)
    {
        Console.WriteLine($"Swithch changed to: {_deleteFolderWithContents}");
    }
}