using System;
using System.IO;
using System.Reflection;

namespace DXVcsTools.DXVcsClient
{
    public class DXVcsRepositoryFactory
    {
        static DXVcsServiceProvider _serviceProvider;
        static readonly object _serviceProviderLock = new object();

        static DXVcsRepositoryFactory()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == typeof(DXVcsServiceProvider).Assembly.FullName)
            {
                return typeof(DXVcsServiceProvider).Assembly;
            }

            //if (args.Name == typeof(IDXVCSService).Assembly.FullName)
            //{
            //    return typeof(IDXVCSService).Assembly;
            //}

            return null;
        }

        public static IDXVcsRepository Create(string serviceUrl)
        {
            if (string.IsNullOrEmpty(serviceUrl))
                throw new ArgumentException("serviceUrl");

            lock (_serviceProviderLock)
            {
                if (_serviceProvider == null)
                {
                    CreateServiceProvider();
                }
            }

            return new DXVcsRepository(_serviceProvider.CreateService(serviceUrl));
        }

        static void CreateServiceProvider()
        {
            AppDomainSetup domainSetup = new AppDomainSetup();
            domainSetup.ApplicationBase = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DXVcsServiceProvider)).Location);

            AppDomain domain = AppDomain.CreateDomain("DXVcsServiceProviderDomain", null, domainSetup);
            _serviceProvider = (DXVcsServiceProvider)domain.CreateInstanceAndUnwrap(
                typeof(DXVcsServiceProvider).Assembly.FullName,
                typeof(DXVcsServiceProvider).FullName,
                false,
                BindingFlags.Public | BindingFlags.Instance,
                null,
                null,
                null,
                null,
                null);
        }
    }
}
