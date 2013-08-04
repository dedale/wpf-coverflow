using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    public partial class FlowControl : UserControl
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
        #endregion
        public FlowControl()
        {
            InitializeComponent();
        }
        public void Load(string imagePath)
        {
            coverList.Clear();
            var imageDir = new DirectoryInfo(imagePath);
            int doneImages = 0;
            foreach (FileInfo image in imageDir.GetFiles("*.jpg"))
            {
                var cover = new Cover(image.FullName, doneImages++);
                coverList.Add(cover);
                visualModel.Children.Add(cover);
            }
        }
        public void GoToNext()
        {
            if (index < coverList.Count - 1)
                UpdateIndex(index + 1);
        }
        public void GoToPrevious()
        {
            if (index > 0)
                UpdateIndex(index - 1);
        }
    }
}
