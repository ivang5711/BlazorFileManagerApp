using BlazorFileManager.Data;
using BlazorFileManager.Models;
using Humanizer;
using Radzen.Blazor;
using Radzen;

namespace BlazorFileManager.Pages;
public partial class Index
{
    private List<FileSystemDisplayItem> _items;
    private WeatherForecast[]? forecasts;
    //private string _driveSelected = string.Empty;
    //private string _selectedDriveSize = string.Empty;
    //private List<string> dirs = new();
    //private List<string> files = new();
    //private List<System.IO.FileInfo> fileDataList = new();
    //private DirectoryInfo? _directoryInfo;

    protected override async Task OnInitializedAsync()
    {

        forecasts = await ForecastService.GetForecastAsync(DateTime.Now);
        var drives = _fileManager.GetAllDrives().ToList();
        _items = new();
        foreach(var drive in drives)
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

        //_driveSelected = drives.First().Name;
        //_selectedDriveSize = drives.First().TotalSize.Bytes().Humanize();


        //_directoryInfo = _fileManager.GetDirectoryInfo(Directory.GetDirectoryRoot(_driveSelected));

        ////fileDataList = _directoryInfo.GetFiles().ToList();

        //foreach(var file in _directoryInfo.GetFiles())
        //{
        //    files.Add(file.Name);
        //}

        //foreach(var directory in _directoryInfo.GetDirectories())
        //{
        //    dirs.Add(directory.Name);
        //}

        ////files.Add(_directoryInfo.GetFiles().First().Name);
        ////dirs.Add(_directoryInfo.GetDirectories().First().Name);
        ///


    }

    string type = "Click";
    bool multiple = true;
    //IEnumerable<FileSystemDisplayItem> employees;
    IList<Tuple<FileSystemDisplayItem, RadzenDataGridColumn<FileSystemDisplayItem>>> selectedCellData = new List<Tuple<FileSystemDisplayItem, RadzenDataGridColumn<FileSystemDisplayItem>>>();
    //EventConsole console;

    //protected override async Task OnInitializedAsync()
    //{
    //    await base.OnInitializedAsync();

    //    employees = dbContext.Employees;
    //}

    void Select(DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
    {
        if (!multiple)
        {
            selectedCellData.Clear();
        }

        var cellData = selectedCellData.FirstOrDefault(i => i.Item1 == args.Data && i.Item2 == args.Column);
        if (cellData != null)
        {
            selectedCellData.Remove(cellData);
        }
        else
        {
            selectedCellData.Add(new Tuple<FileSystemDisplayItem, RadzenDataGridColumn<FileSystemDisplayItem>>(args.Data, args.Column));
        }
    }

    void OnCellClick(DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
    {
        if (type == "Click")
        {
            Select(args);
        }
    }

    void OnCellDoubleClick(DataGridCellMouseEventArgs<FileSystemDisplayItem> args)
    {
        if (type != "Click")
        {
            Select(args);
        }


        Console.WriteLine("I just double clicked!");

        var g = args.Data.FullName;

        GoToSubFolder(g);
        StateHasChanged();
    }

    void OnCellRender(DataGridCellRenderEventArgs<FileSystemDisplayItem> args)
    {
        if (selectedCellData.Any(i => i.Item1 == args.Data && i.Item2 == args.Column))
        {
            args.Attributes.Add("style", $"background-color: var(--rz-secondary-lighter);");
        }
    }

    private void GoToSubFolder(string path)
    {
        var dirInfo = _fileManager.GetDirectoryInfo(path);
        _items = new();
        var folders = dirInfo.GetDirectories();
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
}
