using System;                       // String, Int32, Console, ArgumentException
using System.Text;                  // Encoding Class
using System.IO;                    // IOException Class
using System.Net.Sockets;           // Socket, SocketException Class
using System.Net;                   // IPAddress, IPEndPoint Class

namespace Test_CSNet {
    class C0206_tcp_echo_client_socket : TestObject {
        public override void OnTest(string[] args) {
            if ((args.Length < 2) || (args.Length > 3)) {   // 파라미터의 개수를 확인한다.
                throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");
            }
            String server = args[0];        // 서버명 혹은 IP 주소
            // 입력된 String 객체를 바이트로 변환한다.
            byte[] byteBuffer = Encoding.ASCII.GetBytes(args[1]);
            // 필요하다면 port 파라미터를 입력하고 없을 경우 기본값을 7로 설정한다.
            int servPort = (args.Length == 3) ? Int32.Parse(args[2]) : 7;

            Socket sock = null;
            try {
                // TCP socket 인스턴스를 생성한다.
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 서버 IPEndPoint 인스턴스를 생성한다.
                // 이는 Resolve() 메소드가 한 개 이상의 IP 주소를 반환함을 전제로 한다.
                IPEndPoint serverEndPoint = new IPEndPoint(Dns.GetHostEntry(server).AddressList[0], servPort);
                // 서버의 해당 포트에 소켓을 접속시킨다.
                sock.Connect(serverEndPoint);
                Console.WriteLine("Connected to server ... sending echo string");
                // 인코딩된 스트링을 서버로 전송한다.
                sock.Send(byteBuffer, 0, byteBuffer.Length, SocketFlags.None);

                Console.WriteLine("Sent {0} bytes to server ... ", byteBuffer.Length);
                int totalBytesRcvd = 0;     // 현재까지 수신된 바이트
                int byteRcvd = 0;           // 최종 수신 때 수신된 바이트
                // 서버로부터 스트링을 다시 읽어온다.
                while (totalBytesRcvd < byteBuffer.Length) {
                    if ((byteRcvd = sock.Receive(byteBuffer, totalBytesRcvd,
                        byteBuffer.Length - totalBytesRcvd, SocketFlags.None)) == 0) {
                        Console.WriteLine("Connection closed premuaturely.");
                        break;
                    }
                    totalBytesRcvd += byteRcvd;
                }
                Console.WriteLine("Received {0} bytes from server: {1}", totalBytesRcvd,
                    Encoding.ASCII.GetString(byteBuffer, 0, totalBytesRcvd));
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            finally {
                sock.Close();
            }
        }
    }
}
