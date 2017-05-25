using System;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;

namespace WindowsService
{
    public interface IFileConcatenationService
    {
        void Concatenate(string inputDirectoryName, Func<bool> isWorkStoped);
    }

    public class FileConcatenationService : IFileConcatenationService
    {
        private readonly IFileAvailabilityService _fileAvailabilityService;
        private const string OutputFile = "out.pdf";
        private const int FileOpenAttemptsCount = 3;

        private readonly string _outputDirectoryName;

        public FileConcatenationService(string outputDirectoryName, IFileAvailabilityService fileAvailabilityService)
        {
            _fileAvailabilityService = fileAvailabilityService;
            _outputDirectoryName = outputDirectoryName;
            DirectoryAvailabilityService.CreateNew(_outputDirectoryName);
        }

        public void Concatenate(string inputDirectoryName, Func<bool> isWorkStoped)
        {
            Document document = new Document();
            Section section = document.AddSection();
            foreach (var file in Directory.EnumerateFiles(inputDirectoryName))
            {
                if (isWorkStoped())
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
