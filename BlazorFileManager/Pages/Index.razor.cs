using BlazorFileManager.Data;
using BlazorFileManager.Models;
using Radzen;
using Radzen.Blazor;

namespace BlazorFileManager.Pages;

public partial class Index
{
    private List<FileSystemDisplayItem>? _items;

    protected override async Task OnInitializedAsync()
    {
        var drives = _fileManager.GetAllDrives().ToList();
        _items = new();
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

    private readonly string type = "Click";
    private readonly bool multiple = true;
    private readonly IList<Tuple<FileSystemDisplayItem, 
        RadzenDataGridColumn<FileSystemDisplayItem>>> selectedCellData = 
            new List<Tuple<FileSystemDisplayItem, 
                RadzenDataGridColumn<FileSystemDisplayItem>>>();

    private void Select(DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
    {
        if (!multiple)
        {
            selectedCellData.Clear();
        }

        var cellData = selectedCellData
            .FirstOrDefault(i => i.Item1 == args.Data && i.Item2 == args.Column);
        if (cellData != null)
        {
            selectedCellData.Remove(cellData);
        }
        else
        {
            selectedCellData.Add(new Tuple<FileSystemDisplayItem, 
                RadzenDataGridColumn<FileSystemDisplayItem>>(args.Data, args.Column));
        }
    }

    private void OnCellClick(DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
    {
        if (type == "Click")
        {
            Select(args);
        }
    }

    private void OnCellDoubleClick(DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
    {
        if (type != "Click")
        {
            Select(args);
        }

        var path = args.Data.FullName;
        GoToSubFolder(path);
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
        var folders = dirInfo.GetDirectories();
        var temp = dirInfo.Parent?.FullName;
        if (_items[0].FullName == path)
        {
            _items = new();
            temp ??= string.Empty;
            _items.Add(CreateParentItem(temp));
            AddAllFoldersToDisplayModel(folders);
            return;
        }

        _items = new();
        temp ??= dirInfo.Root.FullName;
        _items.Add(CreateParentItem(temp));
        AddAllFoldersToDisplayModel(folders);
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