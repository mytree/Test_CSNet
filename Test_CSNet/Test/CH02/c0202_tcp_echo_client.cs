using System;                       // String, Int32, Console, ArgumentException
using System.Text;                  // Encoding class
using System.IO;                    // IOException class
using System.Net.Sockets;           // TcpClient, NetworkStream, SocketException

namespace Test_CSNet
{
    class C0202_tcp_echo_client : TestObject
    {
        public override void OnTest(string[] args)
        {
            if ((args.Length < 2) || (args.Length > 3))     // 파라미터의 개수를 확인한다.
            {
                throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");
            }
            String server = args[0];                        // 서버명 혹은 IP 주소
            // 입력된 String 을 바이트 형태로 변환한다.
            byte[] byteBuffer = Encoding.ASCII.GetBytes(args[1]);
            // 포트 번호가 파라미터로 입력되었는지를 확인, 아닐 경우 7로 사용한다.
            int servPort = (args.Length == 3) ? Int32.Parse(args[2]) : 7;

            TcpClient client = null;
            NetworkStream netStream = null;

            try
            {
                // 서버의 해당 포트에 접속하는 소켓을 생성한다.
                client = new TcpClient(server, servPort);

                Console.WriteLine("Connected to server... sending echo string");
                netStream = client.GetStream();

                // 인코딩된 스트링을 서버로 전송한다.
                netStream.Write(byteBuffer, 0, byteBuffer.Length);

                Console.WriteLine("Sent {0} bytes to server...", byteBuffer.Length);

                int totalBytesRcvd = 0;         // 현재까지 수신된 바이트
                int bytesRcvd = 0;              // 최종 수신 때 수신된 바이트

                // 서버로부터 스트링을 다시 읽어온다.
                while (totalBytesRcvd < byteBuffer.Length)
                {
                    if ((bytesRcvd = netStream.Read(byteBuffer, totalBytesRcvd, byteBuffer.Length - totalBytesRcvd)) == 0)
                    {
                        Console.WriteLine("Connection closed prematurely.");
                        break;
                    }
                    totalBytesRcvd += bytesRcvd;
                }
                Console.WriteLine("Received {0} bytes from server: {1}", totalBytesRcvd,
                    Encoding.ASCII.GetString(byteBuffer, 0, totalBytesRcvd));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                netStream.Close();
                client.Close();
            }
        }
    }
}
