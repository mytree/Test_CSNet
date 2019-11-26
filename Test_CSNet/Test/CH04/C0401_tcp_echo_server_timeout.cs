using System;
using System.Net;
using System.Net.Sockets;

namespace Test_CSNet {
    class C0401_tcp_echo_server_timeout : TestObject {
        private const int BUFSIZE = 32;         // 수신 버퍼의 크기
        private const int BACKLOG = 5;          // 처리 대기 큐의 최고 크기
        private const int TIMELIMIT = 10000;    // 기본 타임아웃 시간(밀리초)

        public override void OnTest(string[] args) {
            if (args.Length > 1) // 파라미터의 개수를 확인한다.
                throw new ArgumentException("Parameters: [<Port>]");
            int servPort = (args.Length == 1) ? Int32.Parse(args[0]) : 7;

            Socket server = null;
            try {
                // 클라이언트 연결 요청을 수락할 소켓을 생성한다.
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Any, servPort));
                server.Listen(BACKLOG);
            }
            catch (SocketException se) {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                Environment.Exit(se.ErrorCode);
            }
            byte[] rcvBuffer = new byte[BUFSIZE];       // 수신 버퍼
            int bytesRcvd;                              // 수신된 바이트 수
            int totalBytesEchoed = 0;                   // 전송한 바이트 수
            for (; ; ) {        // 무한히 실행하면서 연결 요청을 수락하고 처리한다.
                Socket client = null;
                try {
                    client = server.Accept();           // 클라이언트 연결 요청을 수락하고 가져온다.
                    DateTime starttime = DateTime.Now;
                    // 수신 타임아웃을 설정한다.
                    client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TIMELIMIT);
                    Console.Write("Handling client at " + client.RemoteEndPoint + " - ");

                    // 클라이언트가 0의 값을 전송하여 연결을 종료할 때까지 계속 수신한다.
                    totalBytesEchoed = 0;
                    while ((bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None)) > 0) {
                        client.Send(rcvBuffer, 0, bytesRcvd, SocketFlags.None);
                        totalBytesEchoed += bytesRcvd;

                        // 경과 시간을 확인한다.
                        TimeSpan elapsed = DateTime.Now - starttime;
                        if (TIMELIMIT - elapsed.TotalMilliseconds < 0) {
                            Console.WriteLine("Aborting client, timelimit " + TIMELIMIT +
                                "ms exceeded; echoed " + totalBytesEchoed + " bytes");
                            client.Close();
                            throw new SocketException(10060);
                        }

                        // Receive timeout 속성 값을 설정한다.
                        client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout,
                            (int)(TIMELIMIT - elapsed.TotalMilliseconds));
                    }
                    Console.WriteLine("echoed {0} bytes.", totalBytesEchoed);
                    client.Close();     // 소켓을 종료하여 이 클라이언트의 연결을 종료한다.
                }
                catch (SocketException se) {
                    if (se.ErrorCode == 10060) {        // WSATIMEDOUT: 연결 타임아웃
                        Console.WriteLine("Aborting client, timelimit " + TIMELIMIT +
                            "ms exceeded; echoed " + totalBytesEchoed + " bytes");
                    }
                    else {
                        Console.WriteLine(se.ErrorCode + ": " + se.Message);
                    }
                    client.Close();
                }
            }
        }
    }
}
