using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DXVcsTools.Core
{
    public class DXVcsBranchCollection : ConfigurationElementCollection
    {
        public DXVcsBranch this[int index]
        {
            get
            {
                return base.BaseGet(index) as DXVcsBranch;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }

                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DXVcsBranch();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DXVcsBranch)element).Name;
        }
    }
}
