using System.Windows.Media;
namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    public interface IThumbnailManager
    {
        ImageSource GetThumbnail(string host, string path);
    }
}
