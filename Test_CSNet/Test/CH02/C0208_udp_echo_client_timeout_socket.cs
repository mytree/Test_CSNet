using System;                   // String, Int32, Boolean, Console
using System.Text;              // Encoding
using System.Net;               // EndPoint, IPEndPoint 
using System.Net.Sockets;       // Socket, SocketOptionName, SocketOptionLevel

namespace Test_CSNet {
    class C0208_udp_echo_client_timeout_socket : TestObject {
        private const int TIMEOUT = 3000;           // 재전송 타임아웃(밀리초 단위)
        private const int MAAXTRIES = 5;            // 최고 재전송 횟수
        public override void OnTest(string[] args) {
            if ((args.Length < 2) || (args.Length > 3)) {   // 파라미터의 개수를 확인한다.
                throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");
            }
            String server = args[0];        // 서버명 혹은 IP 주소
            // 포트 번호가 파라미터로 입력되었는지 확인, 아닐 경우 7로 사용
            int servPort = (args.Length == 3) ? Int32.Parse(args[2]) : 7;
            // 해당 포트에서 서버와 연결할 소켓을 생성한다.
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // 해당 소켓에 대한 수신 타임아웃을 설정한다.
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TIMEOUT);
            IPEndPoint remoteIPEndPoint = new IPEndPoint(Dns.GetHostEntry(server).AddressList[0], servPort);
            EndPoint remoteEndPoint = (EndPoint)remoteIPEndPoint;

            // 입력된 스트링을 바이트 조합으로 변환한다.
            byte[] sendPacket = Encoding.ASCII.GetBytes(args[1]);
            byte[] rcvPacket = new byte[sendPacket.Length];

            int tries = 0;      // 패킷은 손실될 수 있기 때문에 몇 번은 재전송해야 한다.
            Boolean receivedResponse = false;
            do {
                sock.SendTo(sendPacket, remoteEndPoint);        // 에코 스트링을 재전송한다.
                Console.WriteLine("Sent {0} bytes to the server ...", sendPacket.Length);

                try {
                    // 에코응답을 수신한다.
                    sock.ReceiveFrom(rcvPacket, ref remoteEndPoint);
                    receivedResponse = true;
                }
                catch (SocketException se) {
                    tries++;
                    if (se.ErrorCode == 10060) // WSATIMEDOUT : 연결이 타임아웃 되었다.
                        Console.WriteLine("Time out, {0} more tries...", (MAAXTRIES - tries));
                    else // 타임아웃 이외의 문제가 발생했다면, 이를 출력한다.
                        Console.WriteLine(se.ErrorCode + ": " + se.Message);
                }
            } while ((!receivedResponse) && (tries < MAAXTRIES));
            if (receivedResponse) {
                Console.WriteLine("Received {0} bytes from {1}: {2}", rcvPacket.Length, remoteEndPoint,
                    Encoding.ASCII.GetString(rcvPacket, 0, rcvPacket.Length));
            }
            else
                Console.WriteLine("No response - - giving up.");
            sock.Close();
        }
    }
}
