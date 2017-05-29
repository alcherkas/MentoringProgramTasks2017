using System.IO;
using System.Threading;

namespace WindowsService
{
    public interface IFileAvailabilityService
    {
        bool TryOpen(string fullPath);
    }

    public class FileAvailabilityService : IFileAvailabilityService
    {
        private const int FileOpenAttemptsDefaultCount = 3;
        private readonly int _fileOpenAttemptsCount;

        public FileAvailabilityService(int? attemptsCount = null) =>
            _fileOpenAttemptsCount = attemptsCount ?? FileOpenAttemptsDefaultCount;

        public bool TryOpen(string fullPath)
        {
            for (int attemptNumber = 0; attemptNumber < _fileOpenAttemptsCount; attemptNumber++)
            {
                try
                {
                    var file = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.None);
                    file.Close();
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(3000);
                }
            }

            return false;
        }
    }
}
