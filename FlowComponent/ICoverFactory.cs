namespace Ded.Tutorial.Wpf.CoverFlow.FlowComponent
{
    public interface ICoverFactory
    {
        ICover NewCover(string host, string path, int coverPos, int currentPos);
    }
}
