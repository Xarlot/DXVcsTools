namespace DXVcsTools.Core {
    public interface IProjectItemWrapper {
        bool IsSaved { get; }
        string FullPath { get; }
        void Save();
        void Open();
    }
}