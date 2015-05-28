using System.Collections.Generic;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System.ComponentModel;
using System.Linq.Expressions;
using System;

namespace DXVcsTools.Core {
    public abstract class BindableBaseCore : INotifyPropertyChanged {
        public static string GetPropertyName<T>(Expression<Func<T>> expression) {
            return GetPropertyNameFast(expression);
        }
        internal static string GetPropertyNameFast(LambdaExpression expression) {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null) {
                throw new ArgumentException("MemberExpression is expected in expression.Body", "expression");
            }
            const string vblocalPrefix = "$VB$Local_";
            var member = memberExpression.Member;
            if (
                member.MemberType == System.Reflection.MemberTypes.Field &&
                member.Name != null &&
                member.Name.StartsWith(vblocalPrefix))
                return member.Name.Substring(vblocalPrefix.Length);
            return member.Name;
        }
        static bool CompareValues<T>(T storage, T value) {
            return object.Equals(storage, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, string propertyName, Action changedCallback) {
            if (CompareValues<T>(storage, value))
                return false;
            T oldValue = storage;
            storage = value;
            CallChangedCallBackAndRaisePropertyChanged(propertyName, changedCallback);
            return true;
        }

        void CallChangedCallBackAndRaisePropertyChanged(string propertyName, Action changedCallback) {
            RaisePropertyChanged(propertyName);
            if (changedCallback != null)
                changedCallback();
        }

        protected bool SetProperty<T>(ref T storage, T value,
            string propertyName
        ) {
            return SetProperty<T>(ref storage, value, propertyName, null);
        }
        protected void RaisePropertyChanged(
            string propertyName
            ) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void RaisePropertyChanged() {
            RaisePropertiesChanged(null);
        }
        protected void RaisePropertyChanged<T>(Expression<Func<T>> expression) {
            RaisePropertyChanged(GetPropertyName(expression));
        }
        protected void RaisePropertiesChanged(params string[] propertyNames) {
            if (propertyNames == null) {
                RaisePropertyChanged(string.Empty);
                return;
            }
            foreach (string propertyName in propertyNames) {
                RaisePropertyChanged(propertyName);
            }
        }
        protected void RaisePropertiesChanged<T1, T2>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2) {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
        }
        protected void RaisePropertiesChanged<T1, T2, T3>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3) {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
        }
        protected void RaisePropertiesChanged<T1, T2, T3, T4>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3, Expression<Func<T4>> expression4) {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
            RaisePropertyChanged(expression4);
        }
        protected void RaisePropertiesChanged<T1, T2, T3, T4, T5>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3, Expression<Func<T4>> expression4, Expression<Func<T5>> expression5) {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
            RaisePropertyChanged(expression4);
            RaisePropertyChanged(expression5);
        }
        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression, Action changedCallback) {
            string propertyName = GetPropertyName(expression);
            return SetProperty(ref storage, value, propertyName, changedCallback);
        }
        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression) {
            return SetProperty<T>(ref storage, value, expression, null);
        }

        Dictionary<string, object> propertyBag;
        internal Dictionary<string, object> PropertyBag { get { return propertyBag ?? (propertyBag = new Dictionary<string, object>()); } }
        protected bool SetProperty<T>(Expression<Func<T>> expression, T value) {
            return SetProperty(expression, value, null);
        }
        protected bool SetProperty<T>(Expression<Func<T>> expression, T value, Action changedCallback) {
            string propertyName = GetPropertyName(expression);
            T currentValue = default(T);
            object val;
            if (PropertyBag.TryGetValue(propertyName, out val))
                currentValue = (T)val;
            if (CompareValues<T>(currentValue, value))
                return false;
            PropertyBag[propertyName] = value;
            CallChangedCallBackAndRaisePropertyChanged(propertyName, changedCallback);
            return true;
        }
        protected T GetProperty<T>(Expression<Func<T>> expression) {
            string propertyName = GetPropertyName(expression);
            object val;
            if (PropertyBag.TryGetValue(propertyName, out val))
                return (T)val;
            return default(T);
        }
    }
    public enum MergeState {
        None,
        Success,
        Conflict,
        InProgress,
        TargetFileError,
        CheckOutFileError,
        UnknownError,
    }

    public abstract class ProjectItemBase : BindableBaseCore {
        bool isCheckOut;
        bool isChecked;
        bool isNew;
        MergeState mergeState;
        string name;
        string path;
        protected ProjectItemBase(IEnumerable<ProjectItemBase> children = null) {
            Children = children;
            if (Children != null) {
                foreach (var child in Children)
                    child.Parent = this;
            }
        }

        public ProjectItemBase Parent { get; internal set; }
        public virtual int Priority {
            get { return 0; }
        }
        public bool IsNew {
            get { return isNew; }
            set { SetProperty(ref isNew, value, () => IsNew); }
        }
        public string Path {
            get { return path; }
            set { SetProperty(ref path, value, "Path"); }
        }
        public string Name {
            get { return name; }
            set { SetProperty(ref name, value, "Name"); }
        }
        public bool IsBinary {
            get { return MergeFileHelper.IsBinaryFile(Name); }
        }
        public bool IsChecked {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value, "IsChecked"); }
        }
        public bool IsCheckOut {
            get { return isCheckOut; }
            set { SetProperty(ref isCheckOut, value, "IsCheckOut"); }
        }
        public MergeState MergeState {
            get { return mergeState; }
            set { SetProperty(ref mergeState, value, "MergeState"); }
        }
        public bool IsSaved {
            get { return ItemWrapper.If(x => x.IsSaved).ReturnSuccess(); }
        }
        public IEnumerable<ProjectItemBase> Children { get; private set; }
        public IProjectItemWrapper ItemWrapper { get; set; }
        public string FullPath {
            get {
                try {
                    return ItemWrapper.Return(x => x.FullPath, () => string.Empty);
                }
                catch {}
                return string.Empty;
            }
        }

        public void Save() {
            if (!IsSaved)
                ItemWrapper.Do(x => x.Save());
        }
        public void Open() {
            ItemWrapper.Do(x => x.Open());
        }
    }
}