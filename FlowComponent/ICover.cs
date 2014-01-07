using System.Windows.Media.Media3D;
namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    public interface ICover
    {
        void Animate(int index, bool animate);
        bool Matches(MeshGeometry3D mesh);
        void Destroy();
    }
}
