using BlazorFileManager.Models;
using FileManagerDomain.Exceptions;
using FileManagerDomain.Models;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;

namespace BlazorFileManager.Pages;

public partial class Index
{
    public string? NewDirectoryName { get; set; }
    private CurrentFolderViewModel? _currentFolder = new();
    private readonly string type = "Click";
    private bool isFolderSelected = false;
    private string? _errorMessage = string.Empty;

    private readonly IList<Tuple<FileSystemItemViewModel,
        RadzenDataGridColumn<FileSystemItemViewModel>>> selectedCellData =
            new List<Tuple<FileSystemItemViewModel,
                RadzenDataGridColumn<FileSystemItemViewModel>>>();

    private RadzenButton? button;
    private RadzenButton? button2;
    private Popup? popup;
    private Popup? popup2;
    private bool _deleteFolderWithContents;

    protected override void OnInitialized()
    {
        AddAllDrivesToDisplayModel();
    }

    private void CreateNewFolder(string newDirectoryName)
    {
        string newFolder = System.IO.Path
            .Combine(_currentFolder!.FullPath, newDirectoryName);
        try
        {
            _fileManager.CreateFolder(newFolder);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
        }

        NavigateToSubFolder(_currentFolder.FullPath);
    }

    private void DeleteFolder()
    {
        try
        {
            _fileManager.DeleteFolder(
                selectedCellData[0].Item1.FullName,
                _deleteFolderWithContents);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
        }
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

        NavigateToSubFolder(args.Data.FullName);
    }

    private void NavigateToSubFolder(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _currentFolder = new();
            AddAllDrivesToDisplayModel();
            return;
        }

        DirectoryInformation dirInfo;
        try
        {
            dirInfo = _fileManager.GetDirectoryInformation(path);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }

        var temp = dirInfo.Parent;
        if (_currentFolder!.InnerItems[0].FullName == path)
        {
            temp ??= string.Empty;
        }
        else
        {
            temp ??= dirInfo.Root;
        }

        IEnumerable<DirectoryInformation> directories;

        try
        {
            directories = _fileManager.GetAllInnerDerictoriesInformation(path);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }

        IEnumerable<FileInformation> files;
        try
        {
            files = _fileManager.GetAllInnerFilesInfo(path);
        }
        catch (PathArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }

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

        _currentFolder!.InnerItems.AddRange(temp);
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

        _currentFolder!.InnerItems.AddRange(temp);
    }

    private void AddAllDrivesToDisplayModel()
    {
        IEnumerable<DriveInformation> drives;
        try
        {
            drives = _fileManager.GetAllDrives();
        }
        catch (AccessDeniedException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }
        catch (InnerErrorException ex)
        {
            Console.WriteLine(ex.Message);
            _errorMessage = ex.Message;
            return;
        }

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
        await popup!.CloseAsync();
    }

    private async Task CloseDeleteDirectoryDialog()
    {
        await popup2!.CloseAsync();
    }

    private async Task CreateNewDirectory()
    {
        _errorMessage = null;
        if (NewDirectoryName == null)
        {
            _errorMessage = "Directory name can not be empty.";
            return;
        }

        CreateNewFolder(NewDirectoryName!);
        NewDirectoryName = null;
        await CloseCreateDirectoryDialog();
    }

    private async Task ConfirmDeleteDirectory()
    {
        DeleteFolder();
        await CloseDeleteDirectoryDialog();
        NavigateToSubFolder(_currentFolder!.FullPath);
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
}