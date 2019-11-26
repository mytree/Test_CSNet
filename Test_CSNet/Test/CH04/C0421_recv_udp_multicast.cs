using System;               // Console, Int32, ArgumentException
using System.Net;           // IPAddress, EndPoint, IPEndPoint
using System.Net.Sockets;   // Socket

namespace Test_CSNet {
    class C0421_recv_udp_multicast : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length != 2)   // 파라미터 개수 확인
                throw new ArgumentException("Parameter(s): <Multicast Addr> <Port>");
            IPAddress address = IPAddress.Parse(args[0]);       // 멀티캐스트 주소
            if (!MCIPAddress.isValid(args[0]))
                throw new ArgumentException("Valid MC addr: 224.0.0.0 - 239.255.255.255");
            int port = Int32.Parse(args[1]);    // 멀티캐스트 포트
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                ProtocolType.Udp);      // 멀티캐스트 메시지 수신 소켓

            // 주소 재사용 옵션을 설정한다.
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            // IPEndPoint 인스턴스를 생성하여 이에 결합한다.
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            sock.Bind(ipep);

            // 멀티캐스트 그룹 멤버쉽에 가입한다.
            sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                new MulticastOption(address, IPAddress.Any));
            IPEndPoint receivePoint = new IPEndPoint(IPAddress.Any, 0);
            EndPoint tempReceivePoint = (EndPoint)receivePoint;

            // 데이터그램을 생성하고 수신한다.
            byte[] packet = new byte[ItemQuoteTextConst.MAX_WIRE_LENGTH];
            int length = sock.ReceiveFrom(packet, 0, ItemQuoteTextConst.MAX_WIRE_LENGTH,
                SocketFlags.None, ref tempReceivePoint);

            ItemQuoteDecoderText decoder = new ItemQuoteDecoderText();      // 텍스트 디코딩 작업
            ItemQuote quote = decoder.decode(packet);
            Console.WriteLine(quote);

            // 그룹 멤버쉽을 탈퇴한다.
            sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership,
                new MulticastOption(address, IPAddress.Any));
            sock.Close();
        }
    }
}
