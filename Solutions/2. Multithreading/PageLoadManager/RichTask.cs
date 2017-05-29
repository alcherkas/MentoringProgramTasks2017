using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace PageLoadManager
{
    public class RichTask : NotifyPropertyChanged
    {
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public RichTask(string source, Action taskAction)
        {
            Source = source;
            OnPropertyChanged(nameof(Source));
            Task = Task.Run(taskAction, Token);
            Task.ContinueWith(task => OnPropertyChanged(nameof(Status)));
        }

        [Browsable(false)]
        public Guid Id { get; } = Guid.NewGuid();

        [DisplayName("Source")]
        public string Source { get; }

        [DisplayName("Status")]
        public string Status => GetStatusString(Task.Status);

        [Browsable(false)]
        public Task Task { get; }

        [Browsable(false)]
        public CancellationToken Token => _tokenSource.Token;
        public void Cancel() => _tokenSource.Cancel();

        private static string GetStatusString(TaskStatus taskStatus)
        {
            switch (taskStatus)
            {
                case TaskStatus.RanToCompletion: return "Completed";
                case TaskStatus.Running: return "In progress";
                case TaskStatus.Canceled: return "Cancelled";
                default: return "Unknown";
            }
        }
    }
}
