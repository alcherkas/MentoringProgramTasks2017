using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PageLoadManager
{
    public partial class Form1 : Form
    {
        private BindingList<RichTask> tasks = new BindingList<RichTask>();
        public Form1()
        {
            InitializeComponent();
            tasksGrid.DataSource = tasks;
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            tasks.Add(new RichTask(sourceTextBox.Text, () => Download(sourceTextBox.Text)));
        }

        private void Download(string source)
        {
            Thread.Sleep(5000);
        }
    }

    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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
