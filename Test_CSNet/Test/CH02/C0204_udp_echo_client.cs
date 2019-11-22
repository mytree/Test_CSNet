using System;               // String, Int32, Console Class
using System.Text;          // Encoding Class
using System.Net;           // IPEndPoint Class
using System.Net.Sockets;   // UdpClient, SocketException Class

namespace Test_CSNet {
    class C0204_udp_echo_client : TestObject {
        public override void OnTest(string[] args) {
            if ((args.Length < 2) || (args.Length > 3)) {       // 파라미터 개수 확인
                throw new System.ArgumentException("Parameters: <Server> <Word> [<Port>]");
            }
            String server = args[0];                            // 서버명 또는 IP 주소
            // 포트 번호가 파라미터로 입력되었는지를 확인, 아닐 경우 7로 사용한다.
            int servPort = (args.Length == 3) ? Int32.Parse(args[2]) : 7;

            // 입력된 String 을 바이트 어레이로 전환한다.
            byte[] sendPacket = Encoding.ASCII.GetBytes(args[1]);

            // UdpClient 인스턴스를 생성한다.
            UdpClient client = new UdpClient();

            try {
                // 에코할 스트링을 명시된 호스트 주소 및 포트로 전송한다.
                client.Send(sendPacket, sendPacket.Length, server, servPort);

                Console.WriteLine("Send {0} bytes to the server ...", sendPacket.Length);

                // 이 IPEndPoint 인스턴스는 Receive() 메소드의 호출 이 후 
                //  원격 시스템의 엔드포인트 정보를 저장한다.
                IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // 에코되어 돌아오는 정보를 수신한다.
                byte[] rcvPacket = client.Receive(ref remoteIPEndPoint);

                Console.WriteLine("Received {0} bytes from {1}: {2}",
                    rcvPacket.Length, remoteIPEndPoint, Encoding.ASCII.GetString(rcvPacket, 0, rcvPacket.Length));
            }
            catch (SocketException se) {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
            }
            finally {
                client.Close();
            }
        }
    }
}
