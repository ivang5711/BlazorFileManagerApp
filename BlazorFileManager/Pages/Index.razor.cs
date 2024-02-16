using BlazorFileManager.Data;
using BlazorFileManager.Models;
using Radzen;
using Radzen.Blazor;

namespace BlazorFileManager.Pages;

public partial class Index
{
    private List<FileSystemDisplayItem>? _items;
    private readonly string type = "Click";
    private readonly bool multiple = true;
    private string? _errorMessage = null;
    private readonly IList<Tuple<FileSystemDisplayItem,
        RadzenDataGridColumn<FileSystemDisplayItem>>> selectedCellData =
            new List<Tuple<FileSystemDisplayItem,
                RadzenDataGridColumn<FileSystemDisplayItem>>>();

    protected override async Task OnInitializedAsync()
    {
        _items = new();
        AddAllDrivesToDisplayModel();
    }


    private void Select(DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
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
                new Tuple<FileSystemDisplayItem,
                RadzenDataGridColumn<FileSystemDisplayItem>>(
                    args.Data, args.Column));
        }
    }

    private void OnCellClick(
        DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
    {
        if (type == "Click")
        {
            Select(args);
            _errorMessage = null;
        }
    }

    private void OnCellDoubleClick(
        DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
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
            _errorMessage = "You do not have permission to access this folder";
            Console.WriteLine(ex);
            return;
        }

        _items = new();
        _items.Add(CreateParentItem(temp));
        AddAllFoldersToDisplayModel(directories);
    }

    private void AddAllFoldersToDisplayModel(DirectoryInfo[] folders)
    {
        foreach (var folder in folders)
        {
            _items.Add(new FileSystemDisplayItem()
            {
                Name = folder.Name,
                FullName = folder.FullName,
                Parent = folder.Parent!.FullName,
                Extension = string.Empty,
                Size = 0,
                ModifiedDate = folder.LastWriteTime
            });
        }
    }

    private void AddAllDrivesToDisplayModel()
    {
        var drives = _fileManager.GetAllDrives().ToList();
        foreach (var drive in drives)
        {
            _items.Add(new FileSystemDisplayItem()
            {
                Name = drive.Name,
                FullName = drive.Name,
                Parent = string.Empty,
                Extension = string.Empty,
                Size = drive.TotalSize,
            });
        }
    }

    private FileSystemDisplayItem CreateParentItem(string parent)
    {
        return new FileSystemDisplayItem()
        {
            Name = "..",
            FullName = parent,
            Parent = parent,
            Extension = string.Empty,
            Size = 0,
        };
    }
}