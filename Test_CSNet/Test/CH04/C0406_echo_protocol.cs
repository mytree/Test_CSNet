using System.Collections;           // ArrayList
using System.Threading;             // Thread 
using System.Net.Sockets;           // Socket

namespace Test_CSNet {
    class EchoProtocol : IProtocol {
        public const int BUFSIZE = 32;      // 입출력 버퍼의 바이트 크기
        private Socket clntSock;            // 연결 소켓
        private ILogger logger;             // 로깅 기능

        public EchoProtocol(Socket clntSock, ILogger logger) {
            this.clntSock = clntSock;
            this.logger = logger;
        }
        public void handleclient() {
            ArrayList entry = new ArrayList();
            entry.Add("Client address and port = " + clntSock.RemoteEndPoint);
            entry.Add("Thread = " + Thread.CurrentThread.GetHashCode());
            try {
                // SocketException 예외 상황으로 클라이언트와의 연결이 종료될 때까지 수신한다.
                int recvMsgSize;                // 수신된 메시지 크기
                int totalBytesEchoed = 0;       // 클라이언트로부터 수신한 바이트 수
                byte[] rcvBuffer = new byte[BUFSIZE];   // 수신 버퍼

                // 클라이언트가 0의 값을 전송하여 연결을 종료할 때까지 계속 수신한다.
                try {
                    while ((recvMsgSize = clntSock.Receive(rcvBuffer, 0, rcvBuffer.Length,
                        SocketFlags.None)) > 0) {
                            clntSock.Send(rcvBuffer, 0, recvMsgSize, SocketFlags.None);
                            totalBytesEchoed += recvMsgSize;
                    }
                }
                catch (SocketException se) {
                    entry.Add(se.ErrorCode + ": " + se.Message);
                }
                entry.Add("Client finished; echoed " + totalBytesEchoed + " bytes.");
            }
            catch (SocketException se) {
                entry.Add(se.ErrorCode + ": " + se.Message);
            }
            clntSock.Close();
            logger.writeEntry(entry);
        }
    }
}
