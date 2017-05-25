using System;
using System.IO;
using System.Threading;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace WindowsService
{
    class FileConcatenationService
    {
        private const string OutputFile = "out.pdf";
        private const int FileOpenAttemptsCount = 3;

        private readonly FileSystemWatcher _watcher;

        private readonly string _inputDirectoryName;
        private readonly string _outputDirectoryName;

        private readonly Thread _workingThread;
        private readonly ManualResetEvent _workStop;
        private readonly AutoResetEvent _newFile;

        public FileConcatenationService(string inputDirectoryName, string outputDirectoryName)
        {
            _inputDirectoryName = inputDirectoryName;
            _outputDirectoryName = outputDirectoryName;

            if (!Directory.Exists(inputDirectoryName))
                Directory.CreateDirectory(inputDirectoryName);

            if (!Directory.Exists(outputDirectoryName))
                Directory.CreateDirectory(outputDirectoryName);

            _workingThread = new Thread(WorkProc);
            _workStop = new ManualResetEvent(false);
            _newFile = new AutoResetEvent(false);

            _watcher = new FileSystemWatcher(inputDirectoryName);
            _watcher.Created += FileCreated;
        }

        private void WorkProc()
        {
            do
            {
                Document document = new Document();
                Section section = document.AddSection();
                foreach (var file in Directory.EnumerateFiles(_inputDirectoryName))
                {
                    if (_workStop.WaitOne(TimeSpan.Zero))
                    {
                        return;
                    }

                    if (TryOpen(file, FileOpenAttemptsCount))
                    {
                        CreatePdf(file, document, section);
                    }
                }

                var render = new PdfDocumentRenderer();
                render.Document = document;
                render.RenderDocument();
                render.Save(Path.Combine(_outputDirectoryName, OutputFile));
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { _workStop, _newFile }) != 0);
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            _newFile.Set();
        }

        private bool TryOpen(string fullPath, int fileOpenAttemptsCount)
        {
            for (int attemptNumber = 0; attemptNumber < fileOpenAttemptsCount; attemptNumber++)
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

        public void CreatePdf(string file, Document document, Section section)
        {
            var img = section.AddImage(file);

            img.RelativeHorizontal = RelativeHorizontal.Page;
            img.RelativeVertical = RelativeVertical.Page;

            img.Top = 0;
            img.Left = 0;

            img.Height = document.DefaultPageSetup.PageHeight;
            img.Width = document.DefaultPageSetup.PageWidth;

            section.AddPageBreak();
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
    }
}
