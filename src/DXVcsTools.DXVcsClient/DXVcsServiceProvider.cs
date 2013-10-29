using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using DXVCS;

namespace DXVcsTools.DXVcsClient {
    [ServiceContract]
    class DXVcsServiceProvider : MarshalByRefObject {
        static bool isServiceRegistered;
        IDXVCSService service;
        class Factory : ChannelFactory<IDXVCSService> {
            public Factory(ServiceEndpoint endpoint) : base(endpoint) { }
            protected override void ApplyConfiguration(string configurationName) {
            }
        }
        public IDXVCSService CreateService(string serviceUrl) {
            if (isServiceRegistered) {
                try {
                    int version = service.GetServiceVersion();
                    return service;
                }
                catch {
                }
            }
            EndpointAddress myEndpointAddress = new EndpointAddress(new Uri(serviceUrl), new SpnEndpointIdentity(String.Empty));
            ServiceEndpoint point = GZipMessageEncodingBindingElement.CreateEndpoint(myEndpointAddress);
            ChannelFactory<IDXVCSService> factory = new Factory(point);
            factory.Credentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Identification;
            IDXVCSService newService = factory.CreateChannel();
            IContextChannel newChannel = (IContextChannel)newService;
            newChannel.OperationTimeout = TimeSpan.MaxValue;
            newChannel.Open();
            service = newService;
            isServiceRegistered = true;
            service = new ServiceWrapper(newService);
            return service;
        }

        public override object InitializeLifetimeService() {
            return null; // lease never expires
        }
    }
}