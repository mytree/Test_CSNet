using System;               // Int32, ArgumentException
using System.Net;           // IPAddress, IPEndPoint
using System.Net.Sockets;   // Socket

namespace Test_CSNet {
    class C0420_send_udp_multicast : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length < 2 || args.Length > 3) // 파라미터 개수 확인
                throw new ArgumentException("Parameter(s): <Multicast Addr> <Port> [<TTL>]");
            IPAddress destAddr = IPAddress.Parse(args[0]);      // 목적지 주소
            if (!MCIPAddress.isValid(args[0]))
                throw new ArgumentException("Valid MC addr: 224.0.0.0 - 239.255.255.255");
            int destPort = Int32.Parse(args[1]);    // 목적지 포트
            int TTL;        // 데이터그램이 유효한 시간(TTL)
            if (args.Length == 3)
                TTL = Int32.Parse(args[2]);
            else
                TTL = 1;        // 기본 TTL 수치
            ItemQuote quote = new ItemQuote(123456789098765L, "5mm Super Widgets",
                1000, 12999, true, false);
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //TTL 수치를 설정한다.
            sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, TTL);

            ItemQuoteEncoderText encoder = new ItemQuoteEncoderText();  // 텍스트 인코딩
            byte[] codedQuote = encoder.encode(quote);

            // IP 엔드포인트 클래스 인스턴스를 생성한다.
            IPEndPoint ipep = new IPEndPoint(destAddr, destPort);

            // 패킷을 생성하고 전송한다.
            sock.SendTo(codedQuote, 0, codedQuote.Length, SocketFlags.None, ipep);
            sock.Close();
        }
    }
}
