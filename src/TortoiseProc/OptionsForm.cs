using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DXVcsTools.DXVcsClient;

namespace TortoiseProc {
    public partial class OptionsForm : Form {
        public SpacesAction SpacesAction {
            get {
                if(rbCompare.Checked)
                    return SpacesAction.Compare;
                if(rbIgnoreChange.Checked)
                    return SpacesAction.IgnoreChange;
                return SpacesAction.IgnoreAll;
            }
            set {
                if(value == SpacesAction.Compare)
                    rbCompare.Checked = true;
                else if(value == SpacesAction.IgnoreChange)
                    rbIgnoreChange.Checked = true;
                else
                    rbIgnoreAll.Checked = true;
            }
        }

        public OptionsForm() {
            InitializeComponent();
        }
    }
}
