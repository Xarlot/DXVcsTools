namespace DXVcsTools.Core {
    public interface IProjectItemWrapper {
        bool IsSaved { get; }
        string FullPath { get; }
        void Save();
        void Open();
    }
    public interface IProjectWrapper : IProjectItemWrapper {
        string Name { get; }
        string FullName { get; }
    }
}