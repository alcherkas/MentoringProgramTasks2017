using System;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using System.Threading;

namespace WindowsService
{
    public interface IFileConcatenationService
    { 
        void Start();

        void Stop();

        void Concatenate();
    }

    public class FileConcatenationService : IFileConcatenationService
    {
        private readonly IFileAvailabilityService _fileAvailabilityService;
        private readonly IMessageQueueReceiverService _messageReceiver;
        private const string OutputFile = "out.pdf";
        private const int FileOpenAttemptsCount = 3;

        private readonly string _outputDirectoryName;

        private readonly Thread _thread;

        private bool _isWorkStoped;

        public FileConcatenationService(string outputDirectoryName, IFileAvailabilityService fileAvailabilityService, IMessageQueueReceiverService messageReceiver)
        {
            _fileAvailabilityService = fileAvailabilityService;
            _messageReceiver = messageReceiver;
            _outputDirectoryName = outputDirectoryName;
            DirectoryAvailabilityService.CreateNew(_outputDirectoryName);

            _thread = new Thread(Concatenate);
        }

        public void Start() => _thread.Start();

        public void Stop() => _isWorkStoped = true;

        public void Concatenate()
        {
            while (!_isWorkStoped)
            {
                var inputDirectoryName = _messageReceiver.Receive();

                Document document = new Document();
                Section section = document.AddSection();
                foreach (var file in Directory.EnumerateFiles(inputDirectoryName))
                {
                    if (_isWorkStoped)
                    {
                        return;
                    }

                    if (_fileAvailabilityService.TryOpen(file))
                    {
                        CreatePdf(file, document, section);
                    }

                    CreateDocument(document);
                }
            }
        }

        private void CreateDocument(Document document)
        {
            var render = new PdfDocumentRenderer();
            render.Document = document;
            render.RenderDocument();
            render.Save(Path.Combine(_outputDirectoryName, OutputFile));
        }

        private static void CreatePdf(string file, Document document, Section section)
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
    }
}
