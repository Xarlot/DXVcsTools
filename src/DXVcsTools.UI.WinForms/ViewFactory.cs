namespace DXVcsTools.UI.WinForms {
    public class ViewFactory : IViewFactory {
        #region IViewFactory Members
        public IPortWindowView CreatePortWindow() {
            return new DXPortWindow();
        }

        public IBlameWindowView CreateBlameWindow() {
            return new DXBlameWindow();
        }
        #endregion
    }
}