using System.Net.Sockets;       // Socket

namespace Test_CSNet {
    public interface IProtocolFactory {
        IProtocol createProtocol(Socket clntSock, ILogger logger);
    }
}
