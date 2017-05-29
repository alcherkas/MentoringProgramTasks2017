using System.IO;

namespace WindowsService
{
    public static class DirectoryAvailabilityService
    {
        public static void CreateNew(string inputDirectoryName)
        {
            if (!Directory.Exists(inputDirectoryName))
                Directory.CreateDirectory(inputDirectoryName);
        }
    }
}
