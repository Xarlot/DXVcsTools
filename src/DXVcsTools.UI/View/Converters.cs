using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using DXVcsTools.Core;

namespace DXVcsTools.UI {
    public class ObjectToEnumerableConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return new ObservableCollection<SolutionItem> {(SolutionItem)value};
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class HierarchyToEnumerableConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var solution = (SolutionItem)value;
            if (solution == null)
                return null;
            return GetChildren(solution);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
        IEnumerable<ProjectItemBase> GetChildren(ProjectItemBase root) {
            if (root.Children == null)
                yield break;
            foreach (ProjectItemBase item in root.Children) {
                if (item is FileItem)
                    yield return item;
                foreach (ProjectItemBase subItem in GetChildren(item)) {
                    if (subItem is FileItem)
                        yield return subItem;
                }
            }
        }
    }
}