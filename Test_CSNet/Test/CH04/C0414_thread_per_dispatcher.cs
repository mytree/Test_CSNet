using System.Net.Sockets;           // TcpListener, Socket
using System.Threading;             // Thread

namespace Test_CSNet {
    class ThreadPerDispatcher : IDispatcher {
        public void startDispatching(TcpListener listener, ILogger logger,
            IProtocolFactory protoFactory) {
            // 무한히 실행하면서 연결 요청을 수락할 때마다 새로운 서비스 스레드를 생성한다.
            for (; ; ) {
                try {
                    listener.Start();
                    Socket clntSock = listener.AcceptSocket();      // 연결 요청을 대기하면서 블록을 건다.
                    IProtocol protocol = protoFactory.createProtocol(clntSock, logger);
                    Thread thread = new Thread(new ThreadStart(protocol.handleclient));
                    thread.Start();
                    logger.writeEntry("Created and started Thread = " + thread.GetHashCode());
                }
                catch (System.IO.IOException e) {
                    logger.writeEntry("Exception = " + e.Message);
                }
            }
            // 이 곳으로는 도달하지 않는다.
        }
    }
}
