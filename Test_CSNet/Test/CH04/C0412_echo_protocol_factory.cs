using System.Net.Sockets;           // Socket

namespace Test_CSNet {
    public class EchoProtocolFactory : IProtocolFactory {
        public EchoProtocolFactory() { }
        public IProtocol createProtocol(Socket clntSock, ILogger logger) {
            return new EchoProtocol(clntSock, logger);
        }
    }
}
