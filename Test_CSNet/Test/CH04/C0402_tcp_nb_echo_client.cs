using System;               // String, Environment
using System.Text;          // Encoding
using System.IO;            // IOException
using System.Net;           // IPEndPoint, Dns
using System.Net.Sockets;   // TcpClient, NetworkStream, SocketException
using System.Threading;     // Thread.Sleep

namespace Test_CSNet {
    class C0402_tcp_nb_echo_client : TestObject {
        public override void OnTest(string[] args) {
            if ((args.Length < 2) || (args.Length > 3)) // 파라미터의 개수를 확인한다.
                throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");
            String server = args[0];        // 서버명 혹은 IP 주소
            // 입력된 String 을 바이트 형태로 변환한다.
            byte[] byteBuffer = Encoding.ASCII.GetBytes(args[1]);
            // 포트 번호가 파라미터로 입력되었는지를 확인, 아닐 경우 7로 사용한다.
            int servPort = (args.Length == 3) ? Int32.Parse(args[2]) : 7;
            // Socket 객체를 생성하여 연결한다.
            Socket sock = null;
            try {
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(new IPEndPoint(Dns.GetHostEntry(server).AddressList[0], servPort));
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }
            // 서버로부터 스트링을 다시 읽어온다.
            int totalByteSent = 0;      // 전송한 총 바이트
            int totalBytesRcvd = 0;     // 수신한 총 바이트
            // sock 객체를 넌블로킹 Socket 객체로 변환한다.
            sock.Blocking = false;
            // 모든 바이트가 서버로부터 에코되어 수신될 때까지 무한히 반복한다.
            while (totalBytesRcvd < byteBuffer.Length) {
                // 인코딩된 스트링을 서버로 전송한다.
                if (totalByteSent < byteBuffer.Length) {
                    try {
                        totalByteSent += sock.Send(byteBuffer, totalByteSent, byteBuffer.Length - totalByteSent,
                            SocketFlags.None);
                        Console.WriteLine("Sent a total of {0} bytes to server ...", totalByteSent);
                    }
                    catch (SocketException se) {
                        if (se.ErrorCode == 10035) {        // WSAWOULDBLOCK: 리소스가 일시적으로 사용이 불가능하다.
                            Console.WriteLine("Temporarily unable to send, will retry again later.");
                        }
                        else {
                            Console.WriteLine(se.ErrorCode + ": " + se.Message);
                            sock.Close();
                            Environment.Exit(se.ErrorCode);
                        }
                    }
                }
                try {
                    int bytesRcvd = 0;
                    if ((bytesRcvd = sock.Receive(byteBuffer, totalBytesRcvd, byteBuffer.Length - totalBytesRcvd,
                        SocketFlags.None)) == 0) {
                        Console.WriteLine("Connection closed prematurely.");
                        break;
                    }
                    totalBytesRcvd += bytesRcvd;
                }
                catch (SocketException se) {
                    if (se.ErrorCode == 10035) {    // WSAWOULDBLOCK: 리소스가 일시적으로 사용이 불가능하다.

                    }
                    else {
                        Console.WriteLine(se.ErrorCode + ": " + se.Message);
                        break;
                    }
                }
                doThing();
            }
            
            Console.WriteLine("Received {0} bytes from server: {1}", totalBytesRcvd,
                Encoding.ASCII.GetString(byteBuffer, 0, totalBytesRcvd));
            sock.Close();
        }

        static void doThing() {
            Console.Write(".");
            Thread.Sleep(2000);
        }
    }
}
