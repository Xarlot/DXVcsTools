using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DXVcsTools.Core;
using DXVcsTools.UI;
using DXVcsTools.UI.WinForms;
using EnvDTE;
using EnvDTE80;
using Configuration = System.Configuration.Configuration;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace DXVcsTools.VSIX {
    public class MenuViewModel {
        DTE2 _applicationObject;
        Configuration _configuration;

        public void DoConnect(object application) {
            _applicationObject = (DTE2)application;

            _configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
        }
        public void DoPort() {
            string fileName = null;
            if (!CanHandleActiveDocument(ref fileName))
                return;

            var dxPortConfiguration = ConfigurationHelper.GetSection<DXPortConfiguration>(_configuration, "dxPortConfiguration");
            IViewFactory factory = CreateViewFactory(dxPortConfiguration.UIType);

            var model = new PortWindowModel(fileName, _applicationObject.ActiveDocument.ProjectItem.ContainingProject.FullName, dxPortConfiguration);

            IPortWindowView ui = factory.CreatePortWindow();

            var presenter = new PortWindowPresenter(ui, model);
            presenter.Initialize();

            ui.ShowModal();
        }
        IViewFactory CreateViewFactory(DXPortUIType uiType) {
            if (uiType == DXPortUIType.WinForms)
                return new ViewFactory();

            if (uiType == DXPortUIType.Wpf)
                return new UI.Wpf.ViewFactory();

            throw new ArgumentException("Unexpected value: " + uiType, "uiType");
        }

        string GetClassQualifiedCommandName(string name) {
            return string.Format("{0}.{1}", GetType().FullName, name);
        }

        bool CanHandleActiveDocument(ref string fileName) {
            //if (_applicationObject.ActiveDocument == null) {
            //    MessageBox.Show("No current document.", _addInInstance.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //    return false;
            //}

            //fileName = _applicationObject.ActiveDocument.FullName;
            //var sourceControl = (SourceControl2)_applicationObject.SourceControl;

            //if (!sourceControl.IsItemUnderSCC(fileName)) {
            //    MessageBox.Show(string.Concat("File ", fileName, " is not under source control."), _addInInstance.Name, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //    return false;
            //}

            return true;
        }
    }
}
