using System.IO;
using System.Threading;
using Topshelf.Logging;

namespace WindowsService
{
    public interface IFileMonitoringService
    {
        void Start();
        void Stop();
    }

    public class FileMonitoringService : IFileMonitoringService
    {
        private readonly IMessageQueueSenderService _messageQueueSender;

        private readonly FileSystemWatcher _watcher;

        private readonly string _inputDirectoryName;

        private readonly Thread _workingThread;
        private readonly ManualResetEvent _workStop;
        private readonly AutoResetEvent _newFile;

        public FileMonitoringService()
        {
            _messageQueueSender = new MessageQueueService();

            _workingThread = new Thread(WorkProc);
            _workStop = new ManualResetEvent(false);
            _newFile = new AutoResetEvent(false);
            _inputDirectoryName = "in";

            DirectoryAvailabilityService.CreateNew(_inputDirectoryName);

            _watcher = new FileSystemWatcher(_inputDirectoryName);
            _watcher.Created += FileCreated;
        }

        [LoggingAspect]
        public void Start()
        {
            HostLogger.Get<FileMonitoringService>().Info("Starting");
            _workingThread.Start();
            _watcher.EnableRaisingEvents = true;
        }

        [LoggingAspect]
        public void Stop()
        {
            HostLogger.Get<FileConcatenationService>().Info("Stopping");
            _watcher.EnableRaisingEvents = false;
            _workStop.Set();
            _workingThread.Join();
        }

        private void WorkProc()
        {
            do
            {
                _messageQueueSender.Send(_inputDirectoryName);
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { _workStop, _newFile }) != 0);
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            _newFile.Set();
        }
    }
}
