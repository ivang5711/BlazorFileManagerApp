using BlazorFileManager.Models;
using FileManagerDomain.Exceptions;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;

namespace BlazorFileManager.Pages;

public partial class Index
{
    private readonly char[] _forbiddenSymbols =
        { '\\', '/', '|', '?', '*', '\'', ':', '>', '<' };

    private readonly string[] _forbidenDirectoryNames =
        { "CON", "PRN", "AUX", "NUL", "COM1", "COM2",
        "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4",
        "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

    private const string _directoryNameSymbolValidationErrorMessage =
        "Symbols: < > : \" / \\ | ? * not allowed";

    private const string _directoryNameValidationErrorMessage =
        "Directory names CON, PRN, AUX, NUL, COM0, COM1, COM2, COM3, " +
        "COM4, COM5, COM6, COM7, COM8, COM9, LPT0, LPT1, LPT2, LPT3, " +
        "LPT4, LPT5, LPT6, LPT7, LPT8, LPT9 not allowed";

    private const string _directoryNameLengthValidationErrorMessage =
        "File name maximum length allowed is 260 symbols.";

    private const string _directoryNameEndingValidationErrorMessage =
        "File name can not end with \".\" symbol or whitespace.";

    private string DirectoryNameValidationErrorMessage { get; set; } =
        string.Empty;

    private bool _isFolderSelected = false;
    private bool _deleteFolderWithContents;
    private RadzenButton _addNewFolder = new();
    private RadzenButton _deleteFolder = new();
    private string _base64Image = string.Empty;
    private string _errorMessage = string.Empty;
    private Popup _createNewFolderDialog = new();
    private Popup _deleteFolderConfirmationDialog = new();
    private CurrentFolderViewModel? _currentFolder = new();
    private string[] _textTypesForPreview = Array.Empty<string>();
    private string[] _imageTypesForPreview = Array.Empty<string>();
    private bool popup = false;
    private ValidationModel DirectoryNameValidationModel = new();

    private readonly IList<
        Tuple<FileSystemItemViewModel,
            RadzenDataGridColumn<FileSystemItemViewModel>>> _selectedCellData =
            new List<Tuple<FileSystemItemViewModel,
                RadzenDataGridColumn<FileSystemItemViewModel>>>();

    public string? NewDirectoryName { get; set; }
    public string ViewMessage { get; set; } = string.Empty;
    public string TextFileContents { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        _imageTypesForPreview = _fileManagerClient.ImageTypesForPreview;
        _textTypesForPreview = _fileManagerClient.TextTypesForPreview;
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

        ViewMessage = string.Empty;
    }

    private void DeleteDirectory()
    {
        try
        {
            _fileManagerClient!.DeleteFolder(
                        _selectedCellData[0].Item1.FullName!,
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
        TextFileContents = string.Empty;
        _base64Image = string.Empty;
        var cellData = _selectedCellData.FirstOrDefault(
            i => i.Item1 == args.Data && i.Item2 == args.Column);
        if (cellData != null)
        {
            _selectedCellData.RemoveAt(0);
            _isFolderSelected = false;
            ViewMessage = string.Empty;
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

            string extension = _selectedCellData[0].Item1.Extension!;
            string path = _selectedCellData[0].Item1.FullName!;
            SetUpPreview(extension, path);
        }
    }

    private void SetUpPreview(string extension, string path)
    {
        _isFolderSelected = true;
        if (_currentFolder!.IsRoot)
        {
            ViewMessage = "disk";
            return;
        }

        ViewMessage = string.IsNullOrWhiteSpace(extension) ? "folder" : "file";
        if (_textTypesForPreview.Contains(extension.ToLowerInvariant()))
        {
            ViewMessage = string.Empty;
            TextFileContents = _fileManagerClient.GetTextFileContentsForPreview(path);
        }
        else if (_imageTypesForPreview.Contains(extension.ToLowerInvariant()))
        {
            ViewMessage = string.Empty;
            _base64Image = _fileManagerClient.GetImageDataForPreview(path);
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
        NavigateToDirectory(args.Data.FullName!);
        RefreshDirectoryContents();
    }

    private async Task OnSubmit(ValidationModel model)
    {
        Console.WriteLine("Submitted");
        NewDirectoryName = model.DirectoryName;
        await RequestCreateNewDirectory();
    }

    private void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        Console.WriteLine("InValid submitted");
    }

    private bool ValidateNewEmail(string email)
    {
        if (email.Length >= 260)
        {
            DirectoryNameValidationErrorMessage =
                _directoryNameLengthValidationErrorMessage;
            return false;
        }
        else if (email.EndsWith('.') || email.EndsWith(' '))
        {
            DirectoryNameValidationErrorMessage =
                _directoryNameEndingValidationErrorMessage;
            return false;
        }
        else if (email.IndexOfAny(_forbiddenSymbols) != -1)
        {
            DirectoryNameValidationErrorMessage =
                _directoryNameSymbolValidationErrorMessage;
            return false;
        }
        else if (_forbidenDirectoryNames.Contains(email))
        {
            DirectoryNameValidationErrorMessage =
                _directoryNameValidationErrorMessage;
            return false;
        }

        return true;
    }
}