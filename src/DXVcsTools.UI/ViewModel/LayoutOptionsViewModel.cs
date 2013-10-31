using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Mvvm;

namespace DXVcsTools.UI {
    public class LayoutOptionsViewModel : BindableBase {
        GlyphSize toolbarGlyphSize;
        GlyphSize menuGlyphSize;

        public GlyphSize ToolbarGlyphSize {
            get { return toolbarGlyphSize; }
            set { SetProperty(ref toolbarGlyphSize, value, () => ToolbarGlyphSize); }
        }
        public GlyphSize MenuGlyphSize {
            get { return menuGlyphSize; }
            set { SetProperty(ref menuGlyphSize, value, () => MenuGlyphSize); }
        }
    }
}
