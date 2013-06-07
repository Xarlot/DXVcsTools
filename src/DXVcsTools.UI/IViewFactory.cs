namespace DXVcsTools.UI {
    public interface IViewFactory {
        IPortWindowView CreatePortWindow();
        IBlameWindowView CreateBlameWindow();
    }
}