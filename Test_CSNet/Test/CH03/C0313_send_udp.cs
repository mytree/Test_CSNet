using System;               // String, Console, ArgumentException
using System.Net.Sockets;   // TcpClient, NetworkStream

namespace Test_CSNet {
    class C0313_send_udp : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length != 2 && args.Length != 3)    // 파라미터 개수 확인
                throw new ArgumentException("Parameter(s): <Destination>" + " <Port> [<encoding>]");
            String server = args[0];                    // 목적지 주소
            int destPort = Int32.Parse(args[1]);        // 목적지 포트

            ItemQuote quote = new ItemQuote(1234567890987654L, "5mm Super Widgets",
                100, 12999, true, false);

            UdpClient client = new UdpClient();     // 전송을 위한 UDP 소켓

            ItemQuoteEncoder encoder = (args.Length == 3) ?
                new ItemQuoteEncoderText(args[2]) : new ItemQuoteEncoderText();

            byte[] codedQuote = encoder.encode(quote);

            client.Send(codedQuote, codedQuote.Length, server, destPort);

            client.Close();
        }
    }
}
