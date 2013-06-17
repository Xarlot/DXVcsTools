namespace DXVcsTools.Core {
    public interface IDteWrapper {
        SolutionItem BuildTree();
        void OpenSolution(string path);
    }
}