namespace DXVcsTools.Core {
    public interface IDXVcsBindingInfo {
        string GetProjectBinding(string projectFile, out string server);
    }
}