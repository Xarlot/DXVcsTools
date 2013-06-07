using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DXVcsTools.Core
{
    public class DXPortConfiguration : ConfigurationSection
    {
        const string reviewTargetProperty = "reviewTarget";
        const string checkInTargetProperty = "checkInTarget";
        const string closeAfterMergeProperty = "closeAfterMerge";
        const string diffToolProperty = "diffTool";
        const string uiTypeProperty = "uiType";
        const string blameTypeProperty = "blameType";
        const string branchesProperty = "branches";

        [ConfigurationProperty(reviewTargetProperty, DefaultValue = false)]
        public bool ReviewTarget
        {
            get
            {
                return Convert.ToBoolean(this[reviewTargetProperty]);
            }
        }

        [ConfigurationProperty(checkInTargetProperty, DefaultValue = false)]
        public bool CheckInTarget
        {
            get
            {
                return Convert.ToBoolean(this[checkInTargetProperty]);
            }
        }

        [ConfigurationProperty(closeAfterMergeProperty, DefaultValue = false)]
        public bool CloseAfterMerge
        {
            get
            {
                return Convert.ToBoolean(this[closeAfterMergeProperty]);
            }
        }

        [ConfigurationProperty(diffToolProperty, IsRequired = true)]
        public string DiffTool
        {
            get
            {
                return this[diffToolProperty] as string;
            }
        }

        [ConfigurationProperty(uiTypeProperty, DefaultValue = DXPortUIType.WinForms)]
        public DXPortUIType UIType
        {
            get
            {
                return (DXPortUIType)this[uiTypeProperty];
            }
        }

        [ConfigurationProperty(blameTypeProperty, DefaultValue = DXBlameType.Native)]
        public DXBlameType BlameType
        {
            get
            {
                return (DXBlameType)this[blameTypeProperty];
            }
        }

        [ConfigurationProperty(branchesProperty)]
        public DXVcsBranchCollection Branches
        {
            get
            {
                return this[branchesProperty] as DXVcsBranchCollection;
            }
        }
    }
}
