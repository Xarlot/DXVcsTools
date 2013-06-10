using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DevExpress.Xpf.Mvvm;
using EnvDTE;

namespace DXVcsTools.VSIX {
    public class ToolWindowViewModel : BindableBase, IUpdatableViewModel {
        readonly DTE dte;
        public ToolWindowViewModel(DTE dte) {
            this.dte = dte;
            MergeCommand = new DelegateCommand(Merge, CanMerge);
            MergeAllCommand = new DelegateCommand(MergeAll, CanMergeAll);
        }        
        SolutionItem solutionItem;
        public SolutionItem Solution {
            get { return solutionItem; }
            set { SetProperty(ref solutionItem, value, "Solution"); }
        }
        public DelegateCommand MergeCommand { get; private set; }
        public DelegateCommand MergeAllCommand { get; private set; }
        public void Update() {
            DteWrapper dteWrapper = new DteWrapper(dte);
            Solution = dteWrapper.BuildTree();
        }
        void Merge() {
        }
        bool CanMerge() {
            return true;
        }
        void MergeAll() {
        }
        bool CanMergeAll() {
            return true;
        }
    }
}
