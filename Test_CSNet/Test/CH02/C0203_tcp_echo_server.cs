using System;               // Console, Int32, ArgumentException, Environment class
using System.Net;           // IPAddress
using System.Net.Sockets;   // TcpListener, TcpClient class

namespace Test_CSNet
{
    class C0203_tcp_echo_server : TestObject
    {
        private const int BUFSIZE = 32;         // 수신 버퍼의 크기

        public override void OnTest(string[] args)
        {
            if (args.Length > 1)        // 파라미터의 개수를 확인한다.
                throw new ArgumentException("Parameters: [<Port>]");
            int servPort = (args.Length == 1) ? Int32.Parse(args[0]) : 7;
            TcpListener listener = null;
            try
            {
                // 클라이언트의 연결 요청을 수락할 TcpListener 인스턴스를 생성한다.
                listener = new TcpListener(IPAddress.Any, servPort);
                listener.Start();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                Environment.Exit(se.ErrorCode);
            }
            byte[] rcvBuffer = new byte[BUFSIZE];       // 수신 버퍼
            int bytesRcvd;                              // 현재까지 수신된 바이트 수

            for (; ; )          // 무한히 실행하면서 연결 요청을 수락하고 처리한다.
            {
                TcpClient client = null;
                NetworkStream netStream = null;
                try
                {
                    client = listener.AcceptTcpClient();            // 클라이언트 연결 요청을 수락하고 가져온다.
                    netStream = client.GetStream();
                    Console.Write("Handling client - ");

                    // 클라이언트가 0의 값을 전송하여 연결을 종료할 때까지 계속 수신한다.
                    int totalBytesEchoed = 0;
                    while ((bytesRcvd = netStream.Read(rcvBuffer, 0, rcvBuffer.Length)) > 0)
                    {
                        netStream.Write(rcvBuffer, 0, bytesRcvd);
                        totalBytesEchoed += bytesRcvd;
                    }
                    Console.WriteLine("echoed {0} bytes.", totalBytesEchoed);

                    // 스트림과 소켓을 종료하여 이 클라이언트와의 연결을 종료한다.
                    netStream.Close();
                    client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    netStream.Close();
                }
            }
        }
    }
}
