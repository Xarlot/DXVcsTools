using System;

namespace DXVcsTools.UI.Wpf {
    public class ViewFactory : IViewFactory {
        #region IViewFactory Members
        public IPortWindowView CreatePortWindow() {
            return new DXPortWindow();
        }

        public IBlameWindowView CreateBlameWindow() {
            throw new NotImplementedException();
        }
        #endregion
    }
}