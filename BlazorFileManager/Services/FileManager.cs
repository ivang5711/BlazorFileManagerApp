using System.IO;

namespace BlazorFileManager.Services
{
    public class FileManager
    {
        public List<DriveInfo> GetAllDrives()
        {
            return DriveInfo.GetDrives().ToList();
        }
    }
}
