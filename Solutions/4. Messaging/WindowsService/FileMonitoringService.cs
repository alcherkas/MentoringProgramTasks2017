using System;
using System.IO;
using System.Threading;

namespace WindowsService
{
    public interface IFileMonitoringService
    {
        void Start();
        void Stop();
    }

    public class FileMonitoringService : IFileMonitoringService
    {
        private readonly IFileConcatenationService _fileConcatenationService;

        private readonly FileSystemWatcher _watcher;

        private readonly string _inputDirectoryName;

        private readonly Thread _workingThread;
        private readonly ManualResetEvent _workStop;
        private readonly AutoResetEvent _newFile;

        public FileMonitoringService(IFileConcatenationService fileConcatenationService, string inputDirectoryName)
        {
            _fileConcatenationService = fileConcatenationService;

            _workingThread = new Thread(WorkProc);
            _workStop = new ManualResetEvent(false);
            _newFile = new AutoResetEvent(false);
            _inputDirectoryName = inputDirectoryName;

            DirectoryAvailabilityService.CreateNew(_inputDirectoryName);

            _watcher = new FileSystemWatcher(_inputDirectoryName);
            _watcher.Created += FileCreated;
        }

        public void Start()
        {
            _workingThread.Start();
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _workStop.Set();
            _workingThread.Join();
        }

        private void WorkProc()
        {
            do
            {
                _fileConcatenationService.Concatenate(_inputDirectoryName, () => _workStop.WaitOne(TimeSpan.Zero));
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { _workStop, _newFile }) != 0);
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            _newFile.Set();
        }
    }
}
