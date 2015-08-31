using System;
using System.Globalization;
using System.Windows.Data;
using DevExpress.Mvvm.Native;

namespace DXVcsTools.UI.View {
    public class CriteriaOperatorToCheckedConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string test = value.With(x => x.ToString());
            string check = parameter.With(x => x.ToString());
            return test == check;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
