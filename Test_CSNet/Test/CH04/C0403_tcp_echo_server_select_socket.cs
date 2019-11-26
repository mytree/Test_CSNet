using System;                   // Console, Int32, ArgumentException, Environment
using System.Net;               // IPAddress
using System.Collections;       // ArrayList
using System.Net.Sockets;       // Socket, SocketException

namespace Test_CSNet {
    class C0403_tcp_echo_server_select_socket : TestObject {
        private const int BUFSIZE = 32;             // 수신 버퍼의 크기
        private const int BACKLOG = 5;              // 처리 대기 큐의 최고 크기
        private const int SERVER1_PORT = 8080;      // 첫 번째 에코 서버를 위한 포트
        private const int SERVER2_PORT = 8081;      // 두 번째 에코 서버를 위한 포트
        private const int SERVER3_PORT = 8082;      // 세 번째 에코 서버를 위한 포트
        private const int SELECT_WAIT_TIME = 1000;  // Select() 메소드에서 대기할 시간(마이크로초)

        public override void OnTest(string[] args) {
            Socket server1 = null;
            Socket server2 = null;
            Socket server3 = null;
            try {
                // 클라이언트의 연결 요청을 대기할 소켓을 생성한다.
                server1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server3 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server1.Bind(new IPEndPoint(IPAddress.Any, SERVER1_PORT));
                server2.Bind(new IPEndPoint(IPAddress.Any, SERVER2_PORT));
                server3.Bind(new IPEndPoint(IPAddress.Any, SERVER3_PORT));
                server1.Listen(BACKLOG);
                server2.Listen(BACKLOG);
                server3.Listen(BACKLOG);
            }
            catch (SocketException se) {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                Environment.Exit(se.ErrorCode);
            }
            byte[] rcvBuffer = new byte[BUFSIZE];       // 수신 버퍼
            int bytesRcvd;                              // 수신된 바이트 수
            for (; ; ) {        // 무한히 실행하면서 연결 요청을 수락하고 처리한다.
                Socket client = null;
                // 세 개의 소켓 객체를 가진 ArrayList 객체를 생성한다.
                ArrayList acceptList = new ArrayList();
                acceptList.Add(server1);
                acceptList.Add(server2);
                acceptList.Add(server3);
                try {
                    // Select() 메소드 콜은 목록의 각 소켓의 읽기 가능 여부를 확인한다.
                    Socket.Select(acceptList, null, null, SELECT_WAIT_TIME);
                    // acceptList 객체에는 이제 연결을 대기하는 서버 소켓 객체들로만 채워져 있다.
                    for (int i = 0; i < acceptList.Count; i++) {
                        client = ((Socket)acceptList[i]).Accept();
                        // 클라이언트 연결 요청을 수락하고 가져온다.
                        IPEndPoint localEP = (IPEndPoint)((Socket)acceptList[i]).LocalEndPoint;
                        Console.Write("Server port " + localEP.Port);
                        Console.Write(" - handling client at " + client.RemoteEndPoint + " - ");
                        // 클라이언트가 0의 값을 전송하여 연결을 종료할 때까지 계속 수신한다.
                        int totalBytesEchoed = 0;
                        while ((bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None)) > 0) {
                            client.Send(rcvBuffer, 0, bytesRcvd, SocketFlags.None);
                            totalBytesEchoed += bytesRcvd;
                        }
                        Console.WriteLine("echoed {0} bytes.", totalBytesEchoed);
                        client.Close();     // 소켓을 종료하여 이 클라이언트의 연결을 종료한다.
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    client.Close();
                }
            }
        }
    }
}
