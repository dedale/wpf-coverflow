using System.Windows;
using System.Windows.Input;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    public partial class TestWindow : Window
    {
        private class FileInfoComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                return string.Compare(x.FullName, y.FullName);
            }
        }

        #region Fields
        private readonly PerformanceCounter counter = GetCounter();
        private readonly DispatcherTimer timer = new DispatcherTimer();
        #endregion
        #region Handlers
        private void DoKeyDown(Key key)
        {
            switch (key)
            {
                case Key.Right:
                    flow.GoToNext();
                    break;
                case Key.Left:
                    flow.GoToPrevious();
                    break;
                case Key.PageUp:
                    flow.GoToNextPage();
                    break;
                case Key.PageDown:
                    flow.GoToPreviousPage();
                    break;
            }
            if (flow.Index != Convert.ToInt32(slider.Value))
                slider.Value = flow.Index;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            DoKeyDown(e.Key);
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            flow.Index = Convert.ToInt32(slider.Value);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            long kb = Convert.ToInt64(counter.NextValue() / 1000);
            perfLabel.Content = string.Format("{0,12} KB", kb.ToString("###.###.###"));
        }
        #endregion
        #region Private stuff
        private static PerformanceCounter GetCounter()
        {
            var counter = new PerformanceCounter();
            counter.CategoryName = "Process";
            counter.CounterName = "Private Bytes";
            counter.InstanceName = Process.GetCurrentProcess().ProcessName;
            return counter;
        }
        private void Load(string imagePath)
        {
            var imageDir = new DirectoryInfo(imagePath);
            var images = new List<FileInfo>(imageDir.GetFiles("*.jpg"));
            images.Sort(new FileInfoComparer());
            foreach (FileInfo f in images)
                flow.Add(Environment.MachineName, f.FullName);
        }
        #endregion
        public TestWindow()
        {
            InitializeComponent();
            string path = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"Microsoft\Media Player\Cache d’images\LocalMLS");
            //const string path = @"C:\Windows\Web\Wallpaper\Nature";
            flow.Cache = new ThumbnailManager();
            Load(path);
            slider.Minimum = 0;
            slider.Maximum = flow.Count - 1;
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }
    }
}
