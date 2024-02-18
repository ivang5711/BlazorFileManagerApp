namespace BlazorFileManager.Models
{
    public class FileSystemItemViewModel
    {
        /// <summary>
        /// only item name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// item name with full path
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// item path parent is exists
        /// </summary>
        public string? Parent { get; set; }

        /// <summary>
        /// item extension if exists
        /// </summary>
        public string? Extension { get; set; }

        /// <summary>
        /// size on disk in bytes
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// last modified date
        /// </summary>
        public string? ModifiedDate { get; set; }
    }
}