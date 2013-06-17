namespace DXVcsTools.Core {
    public interface IProjectItemWrapper {
        bool IsSaved { get; }
        void Save();
    }
}