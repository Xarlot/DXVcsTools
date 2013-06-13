using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;

namespace DXVcsTools.VSIX {
    public class VSDevExpressMenuItem {
        #region props
        private string headerCore = string.Empty;
        private string toolTipCore = string.Empty;
        private Image iconCore = null;
        private object VSSourceCore = null;

        public VSDevExpressMenuItem Parent { get; protected set; }

        public string Header {
            get { return headerCore; }
            set {
                if (headerCore == value)
                    return;
                string oldValue = headerCore;
                headerCore = value;
                OnHeaderChanged(oldValue);

            }
        }
        public string ToolTip {
            get { return toolTipCore; }
            set {
                if (toolTipCore == value)
                    return;
                string oldValue = toolTipCore;
                toolTipCore = value;
                OnToolTipChanged(oldValue);
            }
        }
        public Image Icon {
            get { return iconCore; }
            set {
                if (iconCore == value)
                    return;
                Image oldValue = iconCore;
                iconCore = value;
                OnIconChanged(oldValue);

            }
        }
        protected object VSSource {
            get { return VSSourceCore; }
            set {
                if (VSSourceCore == value)
                    return;
                object oldValue = VSSourceCore;
                VSSourceCore = value;
                OnVSSourceChanged(oldValue);

            }
        }
        public List<VSDevExpressMenuItem> itemsInternal = new List<VSDevExpressMenuItem>();
        IEnumerable<VSDevExpressMenuItem> Items {
            get {
                foreach (VSDevExpressMenuItem childItem in itemsInternal) {
                    yield return childItem;
                }
                yield break;
            }
        }
        public event EventHandler Click;
        #endregion
        public VSDevExpressMenuItem() {
        }
        protected internal VSDevExpressMenuItem(CommandBarControl source) {
            VSSourceCore = source;
            if (source != null) {
                if (source.Type == MsoControlType.msoControlButton)
                    ((CommandBarButton)source).Click += OnButtonClick;
                headerCore = source.Caption;
                toolTipCore = source.TooltipText;
            }
            CreateChildrenFromSource();
        }
        public VSDevExpressMenuItem CreateItem() {
            if (VSSource == null)
                return null;
            CommandBarControl commandBarControl = (CommandBarControl)VSSource;
            if (commandBarControl.Type != MsoControlType.msoControlPopup) {
                CommandBar parentCommandBar = commandBarControl.Parent;
                if (parentCommandBar == null)
                    return null;
                int savedIndex = commandBarControl.Index;
                VSSource = parentCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, savedIndex, Type.Missing) as CommandBarPopup;
                commandBarControl.Delete();
            }
            CommandBarPopup commandBarPopup = (CommandBarPopup)VSSource;
            VSDevExpressMenuItem childItem = new VSDevExpressMenuItem();
            childItem.VSSource = ((CommandBarPopup)VSSource).Controls.Add(MsoControlType.msoControlButton, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            itemsInternal.Add(childItem);
            childItem.Parent = this;
            return childItem;
        }
        protected void CreateChildrenFromSource() {
            CommandBarPopup commandBarPopup = VSSource as CommandBarPopup;
            if (commandBarPopup == null)
                return;
            foreach (CommandBarControl control in commandBarPopup.Controls) {
                if (control.Type == MsoControlType.msoControlButton || control.Type == MsoControlType.msoControlPopup) {
                    VSDevExpressMenuItem item = new VSDevExpressMenuItem(control);
                    item.Parent = this;
                    itemsInternal.Add(item);
                }
            }
        }
        VSDevExpressMenuItem GetItemByHeader(string header) {
            foreach (VSDevExpressMenuItem child in itemsInternal) {
                if (child.Header == header)
                    return child;
            }
            return null;
        }
        public VSDevExpressMenuItem CreateOrGetItem(string header) {
            VSDevExpressMenuItem childItem = GetItemByHeader(header);
            if (childItem != null)
                return childItem;
            childItem = CreateItem();
            childItem.Header = header;
            return childItem;
        }
        protected virtual void OnIconChanged(Image oldValue) {
            UpdateIcon();
        }
        protected virtual void OnToolTipChanged(string oldValue) {
            UpdateToolTip();
        }
        protected virtual void OnHeaderChanged(string oldValue) {
            UpdateHeader();
        }
        protected virtual void OnVSSourceChanged(object oldValue) {
            CommandBarControl commandBarControl = oldValue as CommandBarControl;
            if (commandBarControl != null) {
                if (commandBarControl.Type == MsoControlType.msoControlButton)
                    ((CommandBarButton)commandBarControl).Click -= OnButtonClick;
            }
            commandBarControl = VSSource as CommandBarControl;
            if (commandBarControl != null) {
                if (commandBarControl.Type == MsoControlType.msoControlButton)
                    ((CommandBarButton)VSSource).Click += OnButtonClick;
                UpdateHeader();
                UpdateIcon();
                UpdateToolTip();
            }
        }
        void OnButtonClick(CommandBarButton Ctrl, ref bool CancelDefault) {
            RaiseClickEvent();
        }
        protected virtual void RaiseClickEvent() {
            if (Click != null)
                Click(this, new EventArgs());
        }
        protected void UpdateHeader() {
            if (VSSource == null || string.IsNullOrEmpty(Header))
                return;
            ((CommandBarControl)VSSource).Caption = Header;
        }
        protected void UpdateToolTip() {
            if (VSSource == null)
                return;
            ((CommandBarControl)VSSource).TooltipText = ToolTip;
        }
        protected void UpdateIcon() {
            if (VSSource == null)
                return;
            if (((CommandBarControl)VSSource).Type != MsoControlType.msoControlButton)
                return;
            ((CommandBarButton)VSSource).Picture = MenuItemPictureHelper.ConvertImageToPicture(Icon);
        }
    }

    public class VSDevExpressMenu : VSDevExpressMenuItem {
        const string DevExpressMenuName = "DXVcsTools";
        public VSDevExpressMenu(DTE dte) {
            Header = DevExpressMenuName;
            CommandBars commandBars = dte.CommandBars as CommandBars;
            CommandBar mainMenuBar = commandBars["MenuBar"];
            CommandBarPopup devExpressMenu = null;

            foreach (CommandBarControl commandBarControl in mainMenuBar.Controls) {
                if (commandBarControl.Type == MsoControlType.msoControlPopup) {
                    CommandBarPopup commandBarPopup = (CommandBarPopup)commandBarControl;
                    if (commandBarPopup.CommandBar.Name == DevExpressMenuName) {
                        devExpressMenu = commandBarPopup;
                        break;
                    }
                }
            }
            if (devExpressMenu == null)
                devExpressMenu = mainMenuBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarPopup;
            VSSource = devExpressMenu;
            CreateChildrenFromSource();
        }
    }
    public class MenuItemPictureHelper : System.Windows.Forms.AxHost {
        public MenuItemPictureHelper() : base("") { }

        static public stdole.StdPicture LoadImageFromResources(string imageName) {
            Image image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Xpf.CreateLayoutWizard.Images." + imageName + ".png"));
            return ConvertImageToPicture(image);
        }
        static public stdole.StdPicture ConvertImageToPicture(Image image) {
            return (stdole.StdPicture)GetIPictureDispFromPicture(image);
        }
    }
}
