using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using DXVcsTools.Core;

namespace DXVcsTools.UI {
    public class OptionsViewModel {
        public OptionsViewModel() {
        }

        [XmlElement]
        [DefaultValue(false)]
        public bool ReviewTarget { get; set; }
        [XmlElement]
        [DefaultValue(false)]
        public bool CheckInTarget { get; set; }
        [XmlElement]
        [DefaultValue(false)]
        public bool CloseAfterMerge { get; set; }
        [XmlElement]
        public string DiffTool { get; set; }
        [XmlElement]
        public DXBlameType BlameType { get; set; }
        [XmlElement]
        public List<DXVcsBranch> Branches { get; set; }
        [XmlElement]
        public bool UpdateOnShowing { get; set; }
        [XmlElement]
        public string LightThemeName { get; set; }
        [XmlElement]
        public string DarkThemeName { get; set; }
        [XmlElement]
        public string BlueThemeName { get; set; }
    }
}