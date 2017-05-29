using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageLoadManager
{
    public partial class Form1 : Form
    {
        private BindingList<RichTask> tasks = new BindingList<RichTask>();
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        public Form1()
        {
            InitializeComponent();
            tasksGrid.DataSource = tasks;
            PathSelector.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            tasks.Add(new RichTask(sourceTextBox.Text, () => DownloadAsync(sourceTextBox.Text).Wait()));
        }

        private async Task DownloadAsync(string source)
        {
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetStringAsync(source);

                Thread.Sleep(400000);
                WriteContent(GetFullFileName(source), result);
            }
        }

        private string GetFullFileName(string source)
            => Path.Combine(PathSelector.Text, source.Replace("/", string.Empty).Replace(":", string.Empty));

        private void WriteContent(string source, string result)
        {
            try
            {
                _lock.EnterWriteLock();
                File.WriteAllText(source, result);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        private void PathSelector_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var openFileDialog = new FolderBrowserDialog { SelectedPath = PathSelector.Text };
            var result = openFileDialog.ShowDialog();
            if(result == DialogResult.OK) PathSelector.Text = openFileDialog.SelectedPath;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (tasksGrid.SelectedRows.Count <= 0) return;

            var number = tasksGrid.SelectedRows[0].Index;
            var task = tasks[number];
            task.Cancel();
            tasks.RemoveAt(number);
        }
    }

    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
