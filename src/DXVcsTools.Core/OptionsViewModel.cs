using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DXVcsTools.Core;

namespace DXVcsTools.Core {
    public class OptionsViewModel {
        const string DxVcsToolsCategory = "DXVcsTools options";

        [Category(DxVcsToolsCategory)]
        [DisplayName("Review target")]
        [Description("Review target")]
        [DefaultValue(false)]
        public bool ReviewTarget { get; set; }

        [Category(DxVcsToolsCategory)]
        [DisplayName("CheckIn target")]
        [Description("CheckIn target")]
        [DefaultValue(false)]
        public bool CheckInTarget { get; set; }

        [Category(DxVcsToolsCategory)]
        [DisplayName("Close after merge")]
        [Description("Close after merge")]
        [DefaultValue(false)]
        public bool CloseAfterMerge { get; set; }

        [Category(DxVcsToolsCategory)]
        [DisplayName("Path to the merger")]
        [Description("Path to the merger")]
        public string DiffTool { get; set; }

        [Category(DxVcsToolsCategory)]
        [DisplayName("Blame type")]
        [Description("Blame type")]
        public DXBlameType BlameType { get; set; }

        [Category(DxVcsToolsCategory)]
        [DisplayName("Branches")]
        [Description("Branches")]
        public List<DXVcsBranch> Branches { get; set; }

        public void Serialize(string path) {
            XmlSerializer xml = new XmlSerializer(typeof(OptionsViewModel));
            using (XmlWriter writer = new XmlTextWriter(path, Encoding.Unicode)) {
                xml.Serialize(writer, this);
            }
        }
        public OptionsViewModel Deserialize(string path) {
            XmlSerializer xml = new XmlSerializer(typeof(OptionsViewModel));
            using (XmlReader reader = new XmlTextReader(path)) {
                return (OptionsViewModel)xml.Deserialize(reader);
            }
            return new OptionsViewModel();
        }
    }
}
