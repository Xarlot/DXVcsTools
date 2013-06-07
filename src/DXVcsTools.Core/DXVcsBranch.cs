using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DXVcsTools.Core
{
    public class DXVcsBranch : ConfigurationElement
    {
        const string nameProperty = "name";
        const string pathProperty = "path";

        [ConfigurationProperty(nameProperty, IsRequired = true)]
        public string Name
        {
            get
            {
                return this[nameProperty] as string;
            }
        }

        [ConfigurationProperty(pathProperty, IsRequired = true)]
        public string Path
        {
            get
            {
                return this[pathProperty] as string;
            }
        }
    }
}
