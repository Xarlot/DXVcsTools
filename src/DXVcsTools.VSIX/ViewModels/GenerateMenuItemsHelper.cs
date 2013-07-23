using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using DXVcsTools.UI.Navigator;
using DXVcsTools.VSIX;
using EnvDTE;

namespace DXVcsTools.ViewModels {
    public class GenerateMenuItemsHelper {
        readonly DTE dte;
        VSDevExpressMenu devExpressMenu;
        readonly DXVcsTools_VSIXPackage package;
        readonly Dictionary<VSDevExpressMenuItem, NavigateItem> menuCache = new Dictionary<VSDevExpressMenuItem, NavigateItem>();
        readonly Dictionary<string, VSDevExpressMenu> menuHierarchy = new Dictionary<string, VSDevExpressMenu>();
        public GenerateMenuItemsHelper(DXVcsTools_VSIXPackage package, DTE dte) {
            this.dte = dte;
            this.package = package;
        }
        public void Release() {
            foreach (var pair in menuHierarchy)
                pair.Value.DeleteItem();
            menuHierarchy.Clear();
            devExpressMenu = null;
        }
        public void GenerateDefault() {
            devExpressMenu = new VSDevExpressMenu(dte, "DXVcsTools");
            menuHierarchy[string.Empty] = devExpressMenu;
            VSDevExpressMenuItem wizardMenu = devExpressMenu.CreateOrGetItem("Show tool window");
            wizardMenu.Click += WizardMenuClick;
            VSDevExpressMenuItem blameMenu = devExpressMenu.CreateOrGetItem("Show blame window");
            blameMenu.Click += BlameMenuClick;
            VSDevExpressMenuItem navigateMenu = devExpressMenu.CreateOrGetItem("Configure navigate menu...");
            navigateMenu.Click += NavigateMenuClick;
        }
        void NavigateMenuClick(object sender, EventArgs e) {
            package.ToolWindowViewModel.ShowNavigationConfig();
        }
        void BlameMenuClick(object sender, EventArgs e) {
            package.ShowBlameWindow();
        }
        void WizardMenuClick(object sender, EventArgs e) {
            package.ShowToolWindow();
        }
        public void GenerateNavigation() {
            Task.Run(new Action(GenerateNavigateMenuAsync));
        }
        void GenerateNavigateMenuAsync() {
            var model = SerializeHelper.DeSerializeNavigationConfig();
            if (model.NavigateItems == null)
                return;
            foreach (var item in model.NavigateItems) {
                GenerateMenuItem(item, model.GetRelativePath);
            }
        }
        void GenerateMenuItem(NavigateItem item, Func<NavigateItem, string> getRelativePath) {
            string relativePath = getRelativePath(item);
            if (string.IsNullOrEmpty(relativePath) || !relativePath.StartsWith("$"))
                return;
            int index = GetDirectorySeparatorIndex(relativePath);
            if (index < 0)
                return;
            string menuName = GetRootMenuName(relativePath);
            var rootMenu = GetRootMenu(menuName);
            GenerateMenuItemContent(rootMenu, item, relativePath);
        }
        string GetRootMenuName(string relativePath) {
            int index = GetDirectorySeparatorIndex(relativePath);
            return relativePath.Substring(0, index + 1).Replace("$", string.Empty).Replace(Path.DirectorySeparatorChar.ToString(), string.Empty).Replace(Path.AltDirectorySeparatorChar.ToString(), string.Empty);
        }
        int GetDirectorySeparatorIndex(string relativePath, int startIndex = 0) {
            int index = relativePath.IndexOf(Path.DirectorySeparatorChar, startIndex);
            if (index < 0)
                index = relativePath.IndexOf(Path.AltDirectorySeparatorChar, startIndex);
            return index;
        }
        void GenerateMenuItemContent(VSDevExpressMenuItem menu, NavigateItem item, string relativePath) {
            int startIndex = 0;
            while (startIndex > -1) {
                int index = GetDirectorySeparatorIndex(relativePath, startIndex + 1);
                if (index < 0)
                    break;
                string name = relativePath.Substring(startIndex + 1, Math.Max(0, index - startIndex - 1));
                startIndex = index;

                if (string.IsNullOrEmpty(name))
                    continue;
                menu = menu.CreateOrGetItem(name, true);
            }
            var menuItem = menu.CreateOrGetItem(item.Name);
            menuCache[menuItem] = item;
            menuItem.Click += menuItem_Click;
        }
        void menuItem_Click(object sender, EventArgs e) {
            NavigateItem item;
            if (menuCache.TryGetValue((VSDevExpressMenuItem)sender, out item)) {
                package.ToolWindowViewModel.NavigateToSolution(item.Path);
            }
        }
        VSDevExpressMenu GetRootMenu(string menuName) {
            VSDevExpressMenu menu;
            if (!menuHierarchy.TryGetValue(menuName, out menu)) {
                menu = new VSDevExpressMenu(dte, menuName);
                menuHierarchy.Add(menuName, menu);
            }
            return menu;
        }

    }
}
