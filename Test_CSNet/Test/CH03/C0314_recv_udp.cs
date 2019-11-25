using System;               // Console, Int32, ArgumentException
using System.Net;           // IPAddress
using System.Net.Sockets;   // TcpListener, TcpClient

namespace Test_CSNet {
    class C0314_recv_udp : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length != 1 && args.Length != 2)   // 파라미터의 개수 확인
                throw new ArgumentException("Parameters: <Port> [<encoding>]");

            int port = Int32.Parse(args[0]);        // 수신 포트

            UdpClient client = new UdpClient(port);  // 수신을 위한 UDP 소켓

            byte[] packet = new byte[ItemQuoteTextConst.MAX_WIRE_LENGTH];
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, port);

            packet = client.Receive(ref remoteIPEndPoint);

            ItemQuoteDecoderText decoder = (args.Length == 2) ? // 인코딩 확인
                new ItemQuoteDecoderText(args[1]) : new ItemQuoteDecoderText();

            ItemQuote quote = decoder.decode(packet);
            Console.WriteLine(quote);

            client.Close();
        }
    }
}
