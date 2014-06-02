﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;
using stdole;

namespace DXVcsTools.VSIX {
    public class VsDevExpressMenuItem {
        #region props
        object vsSourceCore;
        string headerCore = string.Empty;
        Image iconCore;
        public List<VsDevExpressMenuItem> ItemsInternal = new List<VsDevExpressMenuItem>();
        string toolTipCore = string.Empty;

        public VsDevExpressMenuItem Parent { get; protected set; }

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
        protected object VsSource {
            get { return vsSourceCore; }
            set {
                if (vsSourceCore == value)
                    return;
                object oldValue = vsSourceCore;
                vsSourceCore = value;
                OnVSSourceChanged(oldValue);
            }
        }
        public object Tag { get; set; }

        public event EventHandler Click;
        #endregion

        public VsDevExpressMenuItem() {
        }
        protected internal VsDevExpressMenuItem(CommandBarControl source) {
            vsSourceCore = source;
            if (source != null) {
                if (source.Type == MsoControlType.msoControlButton)
                    ((CommandBarButton)source).Click += OnButtonClick;
                headerCore = source.Caption;
                toolTipCore = source.TooltipText;
            }
            CreateChildrenFromSource();
        }
        public void DeleteItem() {
            var commandBarControl = (CommandBarControl)VsSource;
            commandBarControl.Delete();
        }
        public VsDevExpressMenuItem CreateItem(bool isPopup) {
            if (VsSource == null)
                return null;
            var commandBarControl = (CommandBarControl)VsSource;
            if (commandBarControl.Type != MsoControlType.msoControlPopup) {
                CommandBar parentCommandBar = commandBarControl.Parent;
                if (parentCommandBar == null)
                    return null;
                int savedIndex = commandBarControl.Index;
                VsSource = parentCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, savedIndex, Type.Missing) as CommandBarPopup;
                commandBarControl.Delete();
            }
            var childItem = new VsDevExpressMenuItem();
            childItem.VsSource = isPopup
                ? ((CommandBarPopup)VsSource).Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, Type.Missing)
                : ((CommandBarPopup)VsSource).Controls.Add(MsoControlType.msoControlButton, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            ItemsInternal.Add(childItem);
            childItem.Parent = this;
            return childItem;
        }
        protected void CreateChildrenFromSource() {
            var commandBarPopup = VsSource as CommandBarPopup;
            if (commandBarPopup == null)
                return;
            foreach (CommandBarControl control in commandBarPopup.Controls) {
                if (control.Type == MsoControlType.msoControlButton || control.Type == MsoControlType.msoControlPopup) {
                    var item = new VsDevExpressMenuItem(control);
                    item.Parent = this;
                    ItemsInternal.Add(item);
                }
            }
        }
        VsDevExpressMenuItem GetItemByHeader(string header) {
            return ItemsInternal.FirstOrDefault(child => child.Header == header);
        }
        public VsDevExpressMenuItem CreateOrGetItem(string header, bool isPopup = false) {
            VsDevExpressMenuItem childItem = GetItemByHeader(header);
            if (childItem != null)
                return childItem;
            childItem = CreateItem(isPopup);
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
            var commandBarControl = oldValue as CommandBarControl;
            if (commandBarControl != null) {
                if (commandBarControl.Type == MsoControlType.msoControlButton)
                    ((CommandBarButton)commandBarControl).Click -= OnButtonClick;
            }
            commandBarControl = VsSource as CommandBarControl;
            if (commandBarControl != null) {
                if (commandBarControl.Type == MsoControlType.msoControlButton)
                    ((CommandBarButton)VsSource).Click += OnButtonClick;
                UpdateHeader();
                UpdateIcon();
                UpdateToolTip();
            }
        }
        void OnButtonClick(CommandBarButton ctrl, ref bool cancelDefault) {
            RaiseClickEvent();
        }
        protected virtual void RaiseClickEvent() {
            if (Click != null)
                Click(this, new EventArgs());
        }
        protected void UpdateHeader() {
            if (VsSource == null || string.IsNullOrEmpty(Header))
                return;
            ((CommandBarControl)VsSource).Caption = Header;
        }
        protected void UpdateToolTip() {
            if (VsSource == null)
                return;
            ((CommandBarControl)VsSource).TooltipText = ToolTip;
        }
        protected void UpdateIcon() {
            if (VsSource == null)
                return;
            if (((CommandBarControl)VsSource).Type != MsoControlType.msoControlButton)
                return;
            ((CommandBarButton)VsSource).Picture = MenuItemPictureHelper.ConvertImageToPicture(Icon);
        }
    }

    public enum VSDevExpressMenuLocation {
        MenuBar,
        AddReferenceRoot,
        AddReferenceItem,
    }
    public class VSDevExpressMenu : VsDevExpressMenuItem {
        const string DevExpressMenuBarLocation = "MenuBar";
        const string DevExpressMenuAddReferenceLocation = "Reference Root";
        const string DevExpressMenuConvertToProjectReference = "Reference Item";
        const string DevExpressMenuName = "DXVcsTools";
        static readonly IDictionary<VSDevExpressMenuLocation, string> MenusCache = new Dictionary<VSDevExpressMenuLocation, string>();

        static VSDevExpressMenu() {
            MenusCache.Add(VSDevExpressMenuLocation.MenuBar, DevExpressMenuBarLocation);
            MenusCache.Add(VSDevExpressMenuLocation.AddReferenceRoot, DevExpressMenuAddReferenceLocation);
            MenusCache.Add(VSDevExpressMenuLocation.AddReferenceItem, DevExpressMenuConvertToProjectReference);
        }

        public VSDevExpressMenu(DTE dte, string menuName = DevExpressMenuName, VSDevExpressMenuLocation location = VSDevExpressMenuLocation.MenuBar, bool directItem = false) {
            Header = menuName;
            var commandBars = dte.CommandBars as CommandBars;
            CommandBar mainMenuBar = commandBars[MenusCache[location]];
            MsoControlType controlType = GetControlType(directItem);
            if (controlType == MsoControlType.msoControlPopup) {
                CommandBarPopup devExpressMenu = null;
                foreach (CommandBarControl commandBarControl in mainMenuBar.Controls) {
                    if (commandBarControl.Type == MsoControlType.msoControlPopup) {
                        var commandBarPopup = (CommandBarPopup)commandBarControl;
                        if (commandBarPopup.CommandBar.Name == menuName) {
                            devExpressMenu = commandBarPopup;
                            break;
                        }
                    }
                }
                if (devExpressMenu == null)
                    devExpressMenu = mainMenuBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarPopup;
                VsSource = devExpressMenu;
                CreateChildrenFromSource();
            }
            else if (controlType == MsoControlType.msoControlButton) {
                foreach (CommandBarControl commandBarControl in mainMenuBar.Controls) {
                    if (commandBarControl.Type == MsoControlType.msoControlButton) {
                        CommandBarButton button = (CommandBarButton)commandBarControl;
                        if (button.Caption == menuName)
                            throw new ArgumentException("button already registerer");
                    }
                }
                var msoButton = mainMenuBar.Controls.Add(MsoControlType.msoControlButton, Type.Missing, Type.Missing, Type.Missing, Type.Missing) as CommandBarButton;
                msoButton.Caption = menuName;
                VsSource = msoButton;
            }
        }
        MsoControlType GetControlType(bool isDirect) {
            return isDirect ? MsoControlType.msoControlButton : MsoControlType.msoControlPopup;
        }
    }

    public class MenuItemPictureHelper : AxHost {
        public MenuItemPictureHelper()
            : base("") {
        }

        public static StdPicture LoadImageFromResources(string imageName) {
            Image image = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.Xpf.CreateLayoutWizard.Images." + imageName + ".png"));
            return ConvertImageToPicture(image);
        }
        public static StdPicture ConvertImageToPicture(Image image) {
            return (StdPicture)GetIPictureDispFromPicture(image);
        }
    }
}