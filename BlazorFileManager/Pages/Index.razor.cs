using BlazorFileManager.Models;
using Radzen;
using Radzen.Blazor;

namespace BlazorFileManager.Pages;

public partial class Index
{
    private List<FileSystemItemViewModel>? _items;
    private CurrentFolderViewModel? _currentFolder;
    private readonly string type = "Click";
    private readonly bool multiple = true;
    private string? _errorMessage = null;
    private readonly IList<Tuple<FileSystemItemViewModel,
        RadzenDataGridColumn<FileSystemItemViewModel>>> selectedCellData =
            new List<Tuple<FileSystemItemViewModel,
                RadzenDataGridColumn<FileSystemItemViewModel>>>();

    protected override async Task OnInitializedAsync()
    {
        _items = new();
        _currentFolder = new();
        AddAllDrivesToDisplayModel();
    }

    private void CreateNewFolder()
    {
        var p = _items[^1];
        var yo = p.Parent;
        var re = Path.Combine(yo, "abc77");
        _fileManager.CreateFolder(re);
        Console.WriteLine("New folder created!");
        GoToSubFolder(_items[^1].Parent);
    }

    private void DeleteFolder()
    {
        Console.WriteLine("A folder deleted!");
    }


    private void Select(DataGridCellMouseEventArgs<FileSystemItemViewModel> args)
    {
        if (!multiple)
        {
            selectedCellData.Clear();
        }

        var cellData = selectedCellData.FirstOrDefault(
            i => i.Item1 == args.Data && i.Item2 == args.Column);
        if (cellData != null)
        {
            selectedCellData.Remove(cellData);
        }
        else
        {
            selectedCellData.Add(
                new Tuple<FileSystemItemViewModel,
                RadzenDataGridColumn<FileSystemItemViewModel>>(
                    args.Data, args.Column));
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
            _items = new();
            _currentFolder = new();
            AddAllDrivesToDisplayModel();
            return;
        }

        var dirInfo = _fileManager.GetDirectoryInfo(path);
        var temp = dirInfo.Parent?.FullName;
        if (_items[0].FullName == path)
        {
            temp ??= string.Empty;
        }
        else
        {
            temp ??= dirInfo.Root.FullName;
        }

        DirectoryInfo[] directories;
        try
        {
            directories = dirInfo.GetDirectories();
        }
        catch (UnauthorizedAccessException ex)
        {
            _errorMessage = "You do not have permission to access this directory";
            Console.WriteLine(ex);
            return;
        }
        catch (DirectoryNotFoundException ex)
        {
            _errorMessage = "Failed to access the requested directory";
            Console.WriteLine(ex);
            return;
        }

        _items = new();
        _currentFolder = new();
        _items.Add(CreateParentItem(temp));
        _currentFolder = new CurrentFolderViewModel()
        {
            Name = _items[0].Name,
            FullPath = _items[0].FullName,
            Parent = _items[0].Parent,
            IsRoot = false,
        };
        AddAllFoldersToDisplayModel(directories);
    }

    private void AddAllFoldersToDisplayModel(DirectoryInfo[] folders)
    {
        foreach (var folder in folders)
        {
            _items.Add(new FileSystemItemViewModel()
            {
                Name = folder.Name,
                FullName = folder.FullName,
                Parent = folder.Parent!.FullName,
                Extension = string.Empty,
                Size = 0,
                ModifiedDate = folder.LastWriteTime
            });
        }

        _currentFolder.InnerItems.AddRange(_items);
    }

    private void AddAllDrivesToDisplayModel()
    {
        var drives = _fileManager.GetAllDrives().ToList();
        foreach (var drive in drives)
        {
            _items.Add(new FileSystemItemViewModel()
            {
                Name = drive.Name,
                FullName = drive.Name,
                Parent = string.Empty,
                Extension = string.Empty,
                Size = drive.TotalSize,
            });
        }

        _currentFolder = new CurrentFolderViewModel()
        {
            IsRoot = true,
        };
        _currentFolder.InnerItems.AddRange(_items);
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
}