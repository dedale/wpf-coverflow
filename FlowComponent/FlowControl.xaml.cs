using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    public partial class FlowControl : UserControl
    {
        #region Fields
        public const int HalfRealizedCount = 7;
        public const int PageSize = HalfRealizedCount;
        private readonly ICoverFactory coverFactory;
        private readonly Dictionary<int, ImageInfo> imageList = new Dictionary<int, ImageInfo>();
        private readonly Dictionary<string, int> labelIndex = new Dictionary<string, int>();
        private readonly Dictionary<int, string> indexLabel = new Dictionary<int, string>();
        private readonly Dictionary<int, ICover> coverList = new Dictionary<int, ICover>();
        private int index;
        private int firstRealized = -1;
        private int lastRealized = -1;
        #endregion
        #region Private stuff
        private void RotateCover(int pos, bool animate)
        {
            if (coverList.ContainsKey(pos))
                coverList[pos].Animate(index, animate);
        }
        private void UpdateIndex(int newIndex)
        {
            if (index != newIndex)
            {
                bool animate = Math.Abs(newIndex - index) < PageSize;
                UpdateRange(newIndex);
                int oldIndex = index;
                index = newIndex;
                if (index > oldIndex)
                {
                    if (oldIndex < firstRealized)
                        oldIndex = firstRealized;
                    for (int i = oldIndex; i <= index; i++)
                        RotateCover(i, animate);
                }
                else
                {
                    if (oldIndex > lastRealized)
                        oldIndex = lastRealized;
                    for (int i = oldIndex; i >= index; i--)
                        RotateCover(i, animate);
                }
                camera.Position = new Point3D(Cover.CoverStep * index, camera.Position.Y, camera.Position.Z);
            }
        }
        private void viewPort_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var rayMeshResult = (RayMeshGeometry3DHitTestResult)VisualTreeHelper.HitTest(viewPort, e.GetPosition(viewPort));
            if (rayMeshResult != null)
            {
                foreach (int i in coverList.Keys)
                {
                    if (!coverList.ContainsKey(i))
                        continue;
                    if (coverList[i].Matches(rayMeshResult.MeshHit))
                    {
                        UpdateIndex(i);
                        break;
                    }
                }
            }
        }
        private void RemoveCover(int pos)
        {
            if (!coverList.ContainsKey(pos))
                return;
            coverList[pos].Destroy();
            coverList.Remove(pos);
        }
        private void UpdateRange(int newIndex)
        {
            int newFirstRealized = Math.Max(newIndex - HalfRealizedCount, 0);
            int newLastRealized = Math.Min(newIndex + HalfRealizedCount, imageList.Count - 1);
            if (lastRealized < newFirstRealized || firstRealized > newLastRealized)
            {
                visualModel.Children.Clear();
                coverList.Clear();
            }
            else if (firstRealized < newFirstRealized)
            {
                for (int i = firstRealized; i < newFirstRealized; i++)
                    RemoveCover(i);
            }
            else if (newLastRealized < lastRealized)
            {
                for (int i = lastRealized; i > newLastRealized; i--)
                    RemoveCover(i);
            }
            for (int i = newFirstRealized; i <= newLastRealized; i++)
            {
                if (!coverList.ContainsKey(i))
                {
                    ICover cover = coverFactory.NewCover(imageList[i].Host, imageList[i].Path, i, newIndex);
                    coverList.Add(i, cover);
                }
            }
            firstRealized = newFirstRealized;
            lastRealized = newLastRealized;
        }
        protected int FirstRealizedIndex
        {
            get { return firstRealized; }
        }
        protected int LastRealizedIndex
        {
            get { return lastRealized; }
        }
        private void Add(ImageInfo info)
        {
            imageList.Add(imageList.Count, info);
            UpdateRange(index);
        }
        #endregion
        public FlowControl()
        {
            InitializeComponent();
            coverFactory = new CoverFactory(visualModel);
        }
        public IThumbnailManager Cache
        {
            set { Cover.Cache = value; }
        }
        public void GoToNext()
        {
            UpdateIndex(Math.Min(index + 1, imageList.Count - 1));
        }
        public void GoToPrevious()
        {
            UpdateIndex(Math.Max(index - 1, 0));
        }
        public void GoToNextPage()
        {
            UpdateIndex(Math.Min(index + PageSize, imageList.Count - 1));
        }
        public void GoToPreviousPage()
        {
            UpdateIndex(Math.Max(index - PageSize, 0));
        }
        public int Count
        {
            get { return imageList.Count; }
        }
        public int Index
        {
            get { return index; }
            set { UpdateIndex(value); }
        }
        public void Add(string host, string imagePath)
        {
            Add(new ImageInfo(host, imagePath));
        }
    }
}
