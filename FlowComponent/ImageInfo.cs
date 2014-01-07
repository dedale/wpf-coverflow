namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    internal class ImageInfo
    {
        public ImageInfo(string host, string path)
        {
            Host = host;
            Path = path;
        }
        public string Host { get; private set; }
        public string Path { get; private set; }
    }
}
