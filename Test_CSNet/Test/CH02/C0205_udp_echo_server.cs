using System;                       // Console, Int32, ArgumentException, Environment Class
using System.Net;                   // IPEndPoint Class
using System.Net.Sockets;           // UdpClient, SocketException Class

namespace Test_CSNet {
    class C0205_udp_echo_server : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length > 1) {      // 파라미터의 개수 확인
                throw new ArgumentException("Parameters: <Port>");
            }
            int servPort = (args.Length == 1) ? Int32.Parse(args[0]) : 7;
            UdpClient client = null;
            try {
                // 클라이언트 요청을 대기할 UdpClient 인스턴스를 명시된 포트에 생성한다.
                client = new UdpClient(servPort);
            }
            catch (SocketException se) {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                Environment.Exit(se.ErrorCode);
            }
            // IPEndPoint 인스턴스를 생성하여 원격 클라이언트의 정보를 저장할 수 있도록
            //  Receive() 메소드에 대한 참조로 전달한다.
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            for (; ; ) {    // 무한 루프를 돌면서 데이터그램을 수신하고 에코한다.
                try {
                    // 에코 데이터그램 패킷의 내용을 포함하는 바이트 어레이를 수신한다.
                    byte[] byteBuffer = client.Receive(ref remoteIPEndPoint);
                    Console.WriteLine("Handling client at " + remoteIPEndPoint + " - ");

                    // 클라이언트로 에코 패킷을 재전송한다.
                    client.Send(byteBuffer, byteBuffer.Length, remoteIPEndPoint);
                    Console.WriteLine("echoed {0} bytes.", byteBuffer.Length);
                }
                catch (SocketException se) {
                    Console.WriteLine(se.ErrorCode + ": " + se.Message);
                }
            }
        }
    }
}
