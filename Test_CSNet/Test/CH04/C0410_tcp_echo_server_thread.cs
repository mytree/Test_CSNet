using System;                           // Int32, ArgumentException
using System.Threading;                 // Thread
using System.Net;                       // IPAddress
using System.Net.Sockets;               // TcpListener, Socket

namespace Test_CSNet {
    class C0410_tcp_echo_server_thread : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length != 1)   // 파라미터의 개수를 확인한다.
                throw new ArgumentException("Parameter(s): <Port>");
            int echoServPort = Int32.Parse(args[0]);        // 서버포트를 지정한다.
            // 클라이언트 연결 요청을 수락할 TcpListener 소켓 객체를 생성한다.
            TcpListener listener = new TcpListener(IPAddress.Any, echoServPort);

            ILogger logger = new ConsoleLogger();       // 콘솔창으로 메시지를 로깅한다.
            listener.Start();

            // 무한히 실행하면서 연결 요청을 수락할 때마다 새로운 서비스 스레드를 생성한다.
            for (; ; ) {
                try {
                    Socket clntSock = listener.AcceptSocket();      // 연결 요청을 대기하면서 블록을 건다.
                    EchoProtocol protocol = new EchoProtocol(clntSock, logger);
                    Thread thread = new Thread(new ThreadStart(protocol.handleclient));
                    thread.Start();
                    logger.writeEntry("Created and started Thread = " + thread.GetHashCode());
                }
                catch (System.IO.IOException e) {
                    logger.writeEntry("Exception = " + e.Message);
                }
            }
            // 이곳으로는 도달하지 않는다.
        }
    }
}
