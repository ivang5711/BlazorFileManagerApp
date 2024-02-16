namespace BlazorFileManager.Models
{
    public class CurrentFolderViewModel
    {
        public string Name { get; set; } = string.Empty;

        public string FullPath { get; set; } = string.Empty;

        public string Parent { get; set; } = string.Empty;

        public bool IsRoot { get; set; }

        public List<FileSystemItemViewModel> InnerItems { get; set; } = new();
    }
}
