using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DXVcsTools.UI.Wpf;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Configuration = System.Configuration.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace DXVcsTools.VSIX {
    public class MenuViewModel {
        DTE applicationObject;
        Configuration configuration;

        public void DoConnect() {
            applicationObject = Package.GetGlobalService(typeof(DTE)) as DTE ;

            configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        }
        public void DoPort() {
            DteWrapper wrapper = new DteWrapper(applicationObject);
            SolutionItem item = wrapper.BuildTree();
            foreach (var item2 in item.Children) {
                foreach (var item3 in item2.Children) {
                    
                }
            }

            string fileName = null;
            if (!CanHandleActiveDocument(ref fileName))
                return;
            
            var dxPortConfiguration = new OptionsViewModel();
            IViewFactory factory = new ViewFactory();

            var model = new PortWindowModel(fileName, applicationObject.ActiveDocument.ProjectItem.ContainingProject.FullName, dxPortConfiguration);

            IPortWindowView ui = factory.CreatePortWindow();

            var presenter = new PortWindowPresenter(ui, model);
            presenter.Initialize();

            ui.ShowModal();
        }

        string GetClassQualifiedCommandName(string name) {
            return string.Format("{0}.{1}", GetType().FullName, name);
        }

        bool CanHandleActiveDocument(ref string fileName) {
            if (applicationObject.ActiveDocument == null) {
                MessageBox.Show("No current document.", "test", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            fileName = applicationObject.ActiveDocument.FullName;
            var sourceControl = (SourceControl2)applicationObject.SourceControl;

            if (!sourceControl.IsItemUnderSCC(fileName)) {
                MessageBox.Show(string.Concat("File ", fileName, " is not under source control."), "test", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }
    }
}
