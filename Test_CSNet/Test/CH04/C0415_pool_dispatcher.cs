using System.Threading;         // Thread
using System.Net.Sockets;       // TcpListener

namespace Test_CSNet {
    class PoolDispatcher : IDispatcher {
        private const int NUMTHREADS = 8;       // 기본 스레드 풀의 크기
        private int numThreads;                 // 하나의 풀에 포함될 스레드의 개수
        public PoolDispatcher() {
            this.numThreads = NUMTHREADS;
        }
        public PoolDispatcher(int numThreads) {
            this.numThreads = NUMTHREADS;
        }
        public void startDispatching(TcpListener listener, ILogger logger, IProtocolFactory protoFactory) {
            // 각각의 반복적 서버를 구동하는 N개의 스레드를 생성한다.
            for (int i = 0; i < numThreads; i++) {
                DispatchLoop dl = new DispatchLoop(listener, logger, protoFactory);
                Thread thread = new Thread(new ThreadStart(dl.rundispatcher));
                thread.Start();
                logger.writeEntry("Created and started Thread = " + thread.GetHashCode());
            }
        }
    }
    class DispatchLoop {
        TcpListener listener;
        ILogger logger;
        IProtocolFactory protoFactory;

        public DispatchLoop(TcpListener listener, ILogger logger, IProtocolFactory protoFactory) {
            this.listener = listener;
            this.logger = logger;
            this.protoFactory = protoFactory;
        }
        public void rundispatcher() {
            // 무한히 실행하면서 연결 요청을 수락하고 처리한다.
            for (; ; ) {
                try {
                    Socket clntSock = listener.AcceptSocket();      // 블록을 걸고서 연결을 대기한다.
                    IProtocol protocol = protoFactory.createProtocol(clntSock, logger);
                    protocol.handleclient();
                }
                catch (SocketException se) {
                    logger.writeEntry("Exception = " + se.Message);
                }
            }
        }
    }
}
