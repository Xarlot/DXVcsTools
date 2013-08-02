using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DXVcsTools.UI.Navigator;
using DXVcsTools.VSIX;
using EnvDTE;

namespace DXVcsTools.ViewModels {
    public class GenerateMenuItemsHelper {
        readonly DTE dte;
        VSDevExpressMenu devExpressMenu;
        readonly DXVcsTools_VSIXPackage package;
        readonly Dictionary<VSDevExpressMenuItem, NavigateItem> rootMenuCache = new Dictionary<VSDevExpressMenuItem, NavigateItem>();
        readonly Dictionary<VSDevExpressMenuItem, NavigateItem> addReferenceMenuCache = new Dictionary<VSDevExpressMenuItem, NavigateItem>();
        readonly Dictionary<string, VSDevExpressMenu> rootMenuHierarchy = new Dictionary<string, VSDevExpressMenu>();
        readonly Dictionary<string, VSDevExpressMenu> addReferenceMenuHierarchy = new Dictionary<string, VSDevExpressMenu>();
        OptionsViewModel Options { get { return package.ToolWindowViewModel.Options; } }

        public GenerateMenuItemsHelper(DXVcsTools_VSIXPackage package, DTE dte) {
            this.dte = dte;
            this.package = package;
        }
        public void Release() {
            foreach (var pair in rootMenuHierarchy)
                pair.Value.DeleteItem();
            rootMenuCache.Clear();
            rootMenuHierarchy.Clear();
            addReferenceMenuCache.Clear();
            addReferenceMenuHierarchy.Clear();
            devExpressMenu = null;
        }
        public void GenerateDefault() {
            devExpressMenu = new VSDevExpressMenu(dte);
            rootMenuHierarchy[string.Empty] = devExpressMenu;
            VSDevExpressMenuItem wizardMenu = devExpressMenu.CreateOrGetItem("Show tool window");
            wizardMenu.Click += WizardMenuClick;
            VSDevExpressMenuItem blameMenu = devExpressMenu.CreateOrGetItem("Show blame window");
            blameMenu.Click += BlameMenuClick;
            if (Options.UseNavigateMenu) {
                VSDevExpressMenuItem navigateMenu = devExpressMenu.CreateOrGetItem("Configure navigate menu...");
                navigateMenu.Click += NavigateMenuClick;
            }
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
        public void GenerateMenus() {
            if (!Options.UseNavigateMenu)
                return;
            if (Options.UpdateNavigateMenuAsync)
                Task.Run(new Action(GenerateMenuAsync));
            else {
                GenerateMenuAsync();
            }
        }
        void GenerateMenuAsync() {
            var model = SerializeHelper.DeSerializeNavigationConfig();
            if (model.NavigateItems == null)
                return;
            GenerateNavigationMenu(model);
            GenerateAddReferenceMenu(model);
        }
        void GenerateAddReferenceMenu(NavigationConfigViewModel model) {
            foreach (var item in model.NavigateItems) {
                if (ShouldGenerateMenuItem(item))
                    GenerateAddReferenceMenuItem(item, model.GetRelativePath);
            }
        }
        bool ShouldGenerateMenuItem(NavigateItem item) {
            return item.UsedForAddReference && ProjectTypeMatch(item);
        }
        void GenerateAddReferenceMenuItem(NavigateItem item, Func<NavigateItem, string> getRelativePath) {
            string relativePath = getRelativePath(item);
            if (string.IsNullOrEmpty(relativePath) || !relativePath.StartsWith("$"))
                return;
            int index = GetDirectorySeparatorIndex(relativePath);
            if (index < 0)
                return;
            string menuName = GetRootMenuName(relativePath);
            relativePath = relativePath.Substring(index, relativePath.Length - index);
            var rootMenu = GetRootAddReferenceMenu(menuName);
            GenerateMenuItemContent(rootMenu, item, relativePath, (menuItem, navItem) => {
                menuItem.Click += AddReferenceMenuItemClick;
                addReferenceMenuCache[menuItem] = navItem;
            });
        }
        void GenerateNavigationMenu(NavigationConfigViewModel model) {
            foreach (var item in model.NavigateItems) {
                if (item.Used)
                    GenerateMenuItem(item, model.GetRelativePath);
            }
        }
        void GenerateMenuItem(NavigateItem item, Func<NavigateItem, string> getRelativePath) {
            string relativePath = getRelativePath(item);
            if (string.IsNullOrEmpty(relativePath) || !relativePath.StartsWith("$"))
                return;
            if (!ProjectTypeMatch(item))
                return;
            int index = GetDirectorySeparatorIndex(relativePath);
            if (index < 0)
                return;
            string menuName = GetRootMenuName(relativePath);
            relativePath = relativePath.Substring(index, relativePath.Length - index);
            var rootMenu = GetRootMenu(menuName);
            GenerateMenuItemContent(rootMenu, item, relativePath, (menuItem, navItem) => {
                menuItem.Click += RootMenuItemClick;
                rootMenuCache[menuItem] = navItem;
            });
        }
        bool ProjectTypeMatch(NavigateItem item) {
            DteWrapper wrapper = new DteWrapper(dte);
            ProjectType type = wrapper.GetProjectType();
            return (type != ProjectType.NoPlatform || type != ProjectType.Unknown) && wrapper.GetProjectType() == item.ProjectType;
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
        void GenerateMenuItemContent(VSDevExpressMenuItem menu, NavigateItem item, string relativePath, Action<VSDevExpressMenuItem, NavigateItem> addClickHandler) {
            int startIndex = 0;
            relativePath = ReduceRelativePath(relativePath);
            while (startIndex > -1) {
                int index = GetDirectorySeparatorIndex(relativePath, startIndex + 1);
                if (index < 0)
                    break;
                string name = relativePath.Substring(startIndex, Math.Max(0, index - startIndex));
                name = name.Replace(Path.DirectorySeparatorChar.ToString(), string.Empty);
                name = name.Replace(Path.AltDirectorySeparatorChar.ToString(), string.Empty);
                if (string.IsNullOrEmpty(name))
                    continue;
                startIndex = index;

                if (string.IsNullOrEmpty(name))
                    continue;
                menu = menu.CreateOrGetItem(name, true);
            }
            var menuItem = menu.CreateOrGetItem(item.Name);
            addClickHandler(menuItem, item);
        }
        string ReduceRelativePath(string path) {
            string previous = string.Empty;
            string result = string.Empty;
            foreach (string str in path.Split(new[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)) {
                if (str == previous)
                    continue;
                result = Path.Combine(result, str);
                previous = str;
            }
            return result;
        }
        void RootMenuItemClick(object sender, EventArgs e) {
            NavigateItem item;
            if (rootMenuCache.TryGetValue((VSDevExpressMenuItem)sender, out item)) {
                package.ToolWindowViewModel.NavigateToSolution(item.Path);
            }
        }
        void AddReferenceMenuItemClick(object sender, EventArgs e) {
            NavigateItem item;
            if (addReferenceMenuCache.TryGetValue((VSDevExpressMenuItem)sender, out item)) {
                AddReferenceHelper helper = new AddReferenceHelper();
                helper.AddReferences(new DteWrapper(dte), item);
            }
        }
        VSDevExpressMenu GetRootAddReferenceMenu(string menuName) {
            VSDevExpressMenu menu;
            if (!addReferenceMenuHierarchy.TryGetValue(menuName, out menu)) {
                menu = new VSDevExpressMenu(dte, menuName, VSDevExpressMenuLocation.AddReferenceRoot);
                addReferenceMenuHierarchy.Add(menuName, menu);
            }
            return menu;
        }
        VSDevExpressMenu GetRootMenu(string menuName) {
            VSDevExpressMenu menu;
            if (!rootMenuHierarchy.TryGetValue(menuName, out menu)) {
                menu = new VSDevExpressMenu(dte, menuName);
                rootMenuHierarchy.Add(menuName, menu);
            }
            return menu;
        }
    }
}
