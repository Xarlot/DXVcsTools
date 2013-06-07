using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using DXVCS;

namespace DXVcsTools.DXVcsClient {
    class DXVcsServiceProvider : MarshalByRefObject {
        static bool channelRegistered;
        IDXVCSService service;

        static void RegisterChannel() {
            var properties = new Hashtable();
            properties.Add("timeout", 200000);
            properties.Add("secure", true);

            var clientSinkProvider = new BinaryClientFormatterSinkProvider();
            clientSinkProvider.Next = new CompressedClientFormatterSinkProvider(false);

            var clientChannel = new TcpChannel(properties, clientSinkProvider, null);
            ChannelServices.RegisterChannel(clientChannel, false);
        }

        public IDXVCSService CreateService(string serviceUrl) {
            if (!channelRegistered) {
                RegisterChannel();
                channelRegistered = true;
            }

            service = new ServiceWrapper((IDXVCSService)Activator.GetObject(typeof(IDXVCSService), serviceUrl));
            return service;
        }

        public override object InitializeLifetimeService() {
            return null; // lease never expires
        }
    }
}