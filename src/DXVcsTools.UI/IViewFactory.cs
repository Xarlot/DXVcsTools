using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using DXVcsTools.Core;

namespace DXVcsTools.UI
{
    public interface IViewFactory
    {
        IPortWindowView CreatePortWindow();
        IBlameWindowView CreateBlameWindow();
    }
}
