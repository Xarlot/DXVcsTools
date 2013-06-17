namespace DXVcsTools.Core {
    public class SolitionEnumeratorHelper {
        IDteWrapper dte;
        SolutionItem solution;
        public SolitionEnumeratorHelper(IDteWrapper dte) {
            this.dte = dte;
            solution = dte.BuildTree();
        }
    }
}