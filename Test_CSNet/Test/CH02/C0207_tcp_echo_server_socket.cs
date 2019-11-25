using System;                       // String, Int32, Console, ArgumentException
using System.Net;                   // IPAddress, IPEndPoint Class
using System.Net.Sockets;           // Socket, SocketException Class


namespace Test_CSNet {
    class C0207_tcp_echo_server_socket : TestObject {
        private const int BUFSIZE = 32;     // 수신 버퍼의 크기
        private const int BACKLOG = 5;      // 연결 큐의 최대 크기

        public override void OnTest(string[] args) {
            if (args.Length > 1) // Test for correct # of args
                throw new ArgumentException("Parameters: [<Port>]");
            int servPort = (args.Length == 1) ? Int32.Parse(args[0]) : 7;
            Socket server = null;
            try {
                // 클라이언트의 연결 요청을 수락할 소켓을 생성한다.
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, servPort));
                server.Listen(BACKLOG);
            }
            catch (SocketException se) {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                Environment.Exit(se.ErrorCode);
            }

            byte[] rcvBuffer = new byte[BUFSIZE];       // Receive buffer
            int bytesRcvd;                              // Received byte count
            for (; ; ) {        // 무한히 실행하면서 연결 요청을 수락하고 처리한다.
                Socket client = null;
                try {
                    client = server.Accept();       // 클라이언트 연결 요청을 수락하고 가져온다.
                    Console.Write("Handling client at " + client.RemoteEndPoint + " - ");
                    // 클라이언트가 0의 값을 전송하여 연결을 종료할 때까지 계속 수신한다.
                    int totalBytesEchoed = 0;
                    while ((bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None)) > 0) {
                        client.Send(rcvBuffer, 0, bytesRcvd, SocketFlags.None);
                        totalBytesEchoed += bytesRcvd;
                    }
                    Console.WriteLine("echoed {0} bytes.", totalBytesEchoed);
                    client.Close();     // 소켓을 종료하여 이 클라이언트와의 연결을 종료한다.
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    client.Close();
                }
            }


        }
    }
}
