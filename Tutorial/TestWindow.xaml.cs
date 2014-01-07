using System.Windows;
using System.Windows.Input;
using System;
using System.IO;
using System.Collections.Generic;
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
        #endregion
        #region Private stuff
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
            //string path = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"Microsoft\Media Player\Cache d’images\LocalMLS");
            const string path = @"C:\Windows\Web\Wallpaper\Nature";
            flow.Cache = new ThumbnailManager();
            Load(path);
            slider.Minimum = 0;
            slider.Maximum = flow.Count - 1;
        }
    }
}
