using System;
using System.Linq;
using System.Reflection;

namespace DXVcsTools.UI.ViewModel {
    public static class AssemblyLoadingGuard {
        static readonly object Locker = new object();
        static bool isInitialized;
        public static void Protect() {
            if (!isInitialized) {
                lock (Locker) {
                    if (!isInitialized) {
                        //default assembly loading engine seems broken by vs 2013
                        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
                        isInitialized = true;
                    }
                }
            }
        }
        static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args) {
            var result = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
            if (result == null && args.Name.Contains("DevExpress") && args.Name.Contains(AssemblyInfo.VSuffixWithoutSeparator))
                result = Assembly.Load(args.Name);
            return result;
        }
    }
}
