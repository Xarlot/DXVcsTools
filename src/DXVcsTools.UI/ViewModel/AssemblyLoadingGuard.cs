using System;
using System.Linq;

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
        static System.Reflection.Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args) {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.FullName == args.Name);
        }
    }
}
