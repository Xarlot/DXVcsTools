using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
