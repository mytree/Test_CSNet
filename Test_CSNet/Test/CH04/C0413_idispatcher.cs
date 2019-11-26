using System.Net.Sockets;           // TcpListener 

namespace Test_CSNet {
    public interface IDispatcher {
        void startDispatching(TcpListener listener, ILogger logger,
            IProtocolFactory protocolFactory);
    }
}
