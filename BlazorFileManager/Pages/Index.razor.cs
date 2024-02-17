using BlazorFileManager.Models;
using FileManagerDomain.Exceptions;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;

namespace BlazorFileManager.Pages;

public partial class Index
{
    public string? NewDirectoryName { get; set; }
    private RadzenButton _addNewFolder = new();
    private RadzenButton _deleteFolder = new();
    private bool _isFolderSelected = false;
    private bool _deleteFolderWithContents;
    private Popup _createNewFolderDialog = new();
    private string _errorMessage = string.Empty;
    private Popup _deleteFolderConfirmationDialog = new();
    private CurrentFolderViewModel? _currentFolder = new();

    private readonly IList<
        Tuple<FileSystemItemViewModel,
            RadzenDataGridColumn<FileSystemItemViewModel>>> _selectedCellData =
            new List<Tuple<FileSystemItemViewModel,
                RadzenDataGridColumn<FileSystemItemViewModel>>>();

    protected override void OnInitialized()
    {
        InitializeRootDirectory();
        RefreshDirectoryContents();
    }

    private void InitializeRootDirectory()
    {
        try
        {
            _fileManagerClient!.AddAllDrivesToDisplayModel();
        }
        catch (AccessDeniedException ex)
        {
            _errorMessage = ex.Message;
        }
        catch (InnerErrorException ex)
        {
            _errorMessage = ex.Message;
        }
    }

    private void RefreshDirectoryContents()
    {
        _currentFolder = _fileManagerClient!.CurrentFolder;
    }

    private async Task OnCreateNewDirectory()
    {
        await JSRuntime.InvokeVoidAsync(
            "eval",
            "setTimeout(function(){ document" +
            ".getElementById('search').focus(); }, 200)");
    }

    private async Task OnDeleteDirectory()
    {
        await JSRuntime.InvokeVoidAsync(
            "eval",
            "setTimeout(function(){ document" +
            ".getElementById('search').focus(); }, 200)");
    }

    private async Task CloseCreateDirectoryDialog()
    {
        await _createNewFolderDialog.CloseAsync();
    }

    private async Task CloseDeleteDirectoryDialog()
    {
        await _deleteFolderConfirmationDialog.CloseAsync();
    }

    private async Task RequestCreateNewDirectory()
    {
        _errorMessage = string.Empty;
        if (NewDirectoryName is not null)
        {
            await CreateNewDirectory();
            return;
        }

        _errorMessage = "Directory name can not be empty.";
    }

    private async Task CreateNewDirectory()
    {
        CreateNewFolder();
        RefreshDirectoryContents();
        NewDirectoryName = null;
        await CloseCreateDirectoryDialog();
    }

    private void CreateNewFolder()
    {
        try
        {
            _fileManagerClient!.CreateNewFolder(NewDirectoryName!);
        }
        catch (PathArgumentException ex)
        {
            _errorMessage = ex.Message;
        }
        catch (InnerErrorException ex)
        {
            _errorMessage = ex.Message;
        }
        catch (AccessDeniedException ex)
        {
            _errorMessage = ex.Message;
        }
    }

    private async Task ConfirmDeleteDirectory()
    {
        DeleteDirectory();
        await CloseDeleteDirectoryDialog();
        NavigateToDirectory(_currentFolder!.FullPath);
        RefreshDirectoryContents();
    }

    private void NavigateToDirectory(string path)
    {
        try
        {
            _fileManagerClient!.NavigateToSubFolder(path);
        }
        catch (PathArgumentException ex)
        {
            _errorMessage = ex.Message;
        }
        catch (InnerErrorException ex)
        {
            _errorMessage = ex.Message;
        }
        catch (AccessDeniedException ex)
        {
            _errorMessage = ex.Message;
        }
    }

    private void DeleteDirectory()
    {
        try
        {
            _fileManagerClient!.DeleteFolder(
                        _selectedCellData[0].Item1.FullName,
                        _deleteFolderWithContents);
        }
        catch (PathArgumentException ex)
        {
            _errorMessage = ex.Message;
        }
        catch (InnerErrorException ex)
        {
            _errorMessage = ex.Message;
        }
        catch (AccessDeniedException ex)
        {
            _errorMessage = ex.Message;
        }
    }

    private void OnCellRender(
        DataGridCellRenderEventArgs<FileSystemItemViewModel> args)
    {
        if (_selectedCellData
            .Any(i => i.Item1 == args.Data && i.Item2 == args.Column))
        {
            args.Attributes.Add("style",
                $"background-color: var(--rz-secondary-lighter);");
        }
    }

    private void Select(DataGridCellMouseEventArgs<FileSystemItemViewModel> args)
    {
        var cellData = _selectedCellData.FirstOrDefault(
            i => i.Item1 == args.Data && i.Item2 == args.Column);
        if (cellData != null)
        {
            _selectedCellData.RemoveAt(0);
            _isFolderSelected = false;
        }
        else
        {
            _selectedCellData.Add(
                new Tuple<FileSystemItemViewModel,
                RadzenDataGridColumn<FileSystemItemViewModel>>(
                    args.Data, args.Column));
            if (_selectedCellData.Count > 1)
            {
                _selectedCellData.RemoveAt(0);
            }

            _isFolderSelected = true;
        }
    }

    private void OnCellClick(
        DataGridCellMouseEventArgs<FileSystemItemViewModel> args)
    {
        Select(args);
        _errorMessage = string.Empty;
    }

    private void OnCellDoubleClick(
        DataGridCellMouseEventArgs<FileSystemItemViewModel> args)
    {
        NavigateToDirectory(args.Data.FullName);
        RefreshDirectoryContents();
    }
}