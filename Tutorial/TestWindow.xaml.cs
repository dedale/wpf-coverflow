using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    public partial class TestWindow : Window
    {
        #region Private stuff
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    flow.GoToNext();
                    break;
                case Key.Left:
                    flow.GoToPrevious();
                    break;
            }
        }
        #endregion
        public TestWindow()
        {
            InitializeComponent();
            //string path = Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), @"Microsoft\Media Player\Cache d’images\LocalMLS");
            const string path = @"C:\Windows\Web\Wallpaper\Nature";
            flow.Load(path);
        }
    }
}
