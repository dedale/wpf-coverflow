using System.Windows.Media.Media3D;
namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    internal class CoverFactory : ICoverFactory
    {
        private readonly ModelVisual3D visualModel;
        public CoverFactory(ModelVisual3D visualModel)
        {
            this.visualModel = visualModel;
        }
        #region ICoverFactory Members
        public ICover NewCover(string host, string path, int coverPos, int currentPos)
        {
            return new Cover(new ImageInfo(host, path), coverPos, currentPos, visualModel);
        }
        #endregion
    }
}
