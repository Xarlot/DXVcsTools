﻿using System;
using System.IO;
using System.Reflection;

namespace DXVcsTools.DXVcsClient {
    public class DXVcsRepositoryFactory {
        static DXVcsServiceProvider serviceProvider;
        static readonly object ServiceProviderLock = new object();

        static DXVcsRepositoryFactory() {
        }

        public static IDXVcsRepository Create(string serviceUrl) {
            if (string.IsNullOrEmpty(serviceUrl))
                throw new ArgumentException("serviceUrl");
            if (serviceProvider == null) {
                lock (ServiceProviderLock) {
                    if (serviceProvider == null) {
                        CreateServiceProvider();
                    }
                }
            }

            return new DXVcsRepository(serviceProvider.CreateService(serviceUrl));
        }

        static void CreateServiceProvider() {
            var domainSetup = new AppDomainSetup();
            domainSetup.ApplicationBase = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DXVcsServiceProvider)).Location);

            AppDomain domain = AppDomain.CreateDomain("DXVcsServiceProviderDomain", null, domainSetup);
            serviceProvider =
                (DXVcsServiceProvider)
                    domain.CreateInstanceAndUnwrap(typeof(DXVcsServiceProvider).Assembly.FullName, typeof(DXVcsServiceProvider).FullName, false, BindingFlags.Public | BindingFlags.Instance, null, null,
                        null, null, null);
        }
    }
}