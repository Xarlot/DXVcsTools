using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using DevExpress.Mvvm;
using DXVcsTools.Core;

namespace DXVcsTools.UI {
    public class OptionsViewModel : BindableBase {
        LayoutOptionsViewModel layoutOptions;

        public OptionsViewModel() {
        }

        public bool EnableAutoUpdate { get; set; }
        public string AutoUpdaterPath { get; set; }
        public int ConfigVersion { get; set; }
        public bool AssignCommandBindings { get; set; }
        public bool ReviewTarget { get; set; }
        public bool CheckInTarget { get; set; }
        public bool CloseAfterMerge { get; set; }
        public string DiffTool { get; set; }
        public DXBlameType BlameType { get; set; }
        public List<DXVcsBranch> Branches { get; set; }
        public bool UpdateOnShowing { get; set; }
        public string LightThemeName { get; set; }
        public string DarkThemeName { get; set; }
        public string BlueThemeName { get; set; }
        public bool UseNavigateMenu { get; set; }
        public bool UpdateNavigateMenuAsync { get; set; }
        public string TortoiseProc { get; set; }

        [Required]
        public LayoutOptionsViewModel LayoutOptions {
            get { return layoutOptions; }
            set {
                SetProperty(ref layoutOptions, value ?? new LayoutOptionsViewModel(), () => LayoutOptions);
            }
        }
    }
}