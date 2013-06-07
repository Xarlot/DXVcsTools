using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using DXVcsTools.Core;

namespace DXVcsTools.UI.WinForms
{
    public class ViewFactory : IViewFactory
    {
        #region IViewFactory Members

        public IPortWindowView CreatePortWindow()
        {
            return new DXPortWindow();
        }

        public IBlameWindowView CreateBlameWindow()
        {
            return new DXBlameWindow();
        }

        #endregion
    }
}
