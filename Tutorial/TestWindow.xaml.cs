using System;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows.Media;
namespace Ded.Tutorial.Wpf.CoverFlow
{
    public partial class TestWindow : Window
    {
        #region Fields
        private int index;
        private readonly List<Cover> coverList = new List<Cover>();
        #endregion
        #region Private stuff
        private void RotateCover(int pos)
        {
            coverList[pos].Animate(index);
        }
        private void UpdateIndex(int newIndex)
        {
            if (index != newIndex)
            {
                int oldIndex = index;
                index = newIndex;
                if (index > oldIndex)
                    for (int i = oldIndex; i <= index; i++)
                        RotateCover(i);
                else
                    for (int i = oldIndex; i >= index; i--)
                        RotateCover(i);
                camera.Position = new Point3D(Cover.CoverStep * index, camera.Position.Y, camera.Position.Z);
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int newIndex = index;
            switch (e.Key)
            {
                case Key.Right:
                    if (newIndex < coverList.Count - 1)
                        newIndex++;
                    break;
                case Key.Left:
                    if (newIndex > 0)
                        newIndex--;
                    break;
            }
            UpdateIndex(newIndex);
        }
        private void viewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var rayMeshResult = (RayMeshGeometry3DHitTestResult)VisualTreeHelper.HitTest(viewPort, e.GetPosition(viewPort));
            if (rayMeshResult != null)
            {
                for (int i = 0; i < coverList.Count; i++)
                {
                    if (coverList[i].Matches(rayMeshResult.MeshHit))
                    {
                        UpdateIndex(i);
                        break;
                    }
                }
            }
        }
        private static string GetCoversPath()
        {
            const string webPath = @"C:\Windows\Web\Wallpaper\Nature";
            if (Directory.Exists(webPath))
                return webPath;
            string localDataPath = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            if (!string.IsNullOrEmpty(localDataPath))
            {
                foreach (string relativePath in new[] {
                    @"Microsoft\Media Player\Art Cache\LocalMLS",
                    @"Microsoft\Media Player\Cache d’images\LocalMLS"
                })
                {
                    string path = Path.Combine(localDataPath, relativePath);
                    if (Directory.Exists(path))
                        return path;
                }
            }
            return @"c:\";
        }
        #endregion
        public TestWindow()
        {
            InitializeComponent();
            var imageDir = new DirectoryInfo(GetCoversPath());
            int doneImages = 0;
            foreach (FileInfo image in imageDir.GetFiles("*.jpg"))
            {
                var cover = new Cover(image.FullName, doneImages++);
                coverList.Add(cover);
                visualModel.Children.Add(cover);
            }
        }
    }
}
