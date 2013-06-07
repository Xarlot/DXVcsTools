using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace DXVcsTools.VSAddIn
{
    static class ConfigurationHelper
    {
        public static T GetSection<T>(Configuration configuration, string sectionName) where T : ConfigurationSection
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentException("sectionName");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve<T>;

            try
            {
                return configuration.GetSection(sectionName) as T;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve<T>;
            }
        }

        static Assembly CurrentDomain_AssemblyResolve<T>(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(typeof(T).Assembly.FullName);

            if (args.Name == assemblyName.Name)
                return typeof(T).Assembly;

            return null;
        }
    }
}
