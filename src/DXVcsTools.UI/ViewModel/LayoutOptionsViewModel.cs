using DevExpress.Xpf.Bars;
using DevExpress.Mvvm;
using DXVcsTools.Core;

namespace DXVcsTools.UI {
    public class LayoutOptionsViewModel : BindableBaseCore {
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
