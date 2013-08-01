using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcsTools.Core;
using DXVcsTools.UI.Navigator;

namespace DXVcsTools.UI.AddReferenceHelper {
    public class AddReferenceHelper {
        public void AddReferences(IDteWrapper dte, NavigateItem item) {
            SolutionParser parser = new SolutionParser(item.Path);
            foreach (var assembly in parser.Parse()) {
                dte.AddReference(assembly);
            }
        }
    }
}
