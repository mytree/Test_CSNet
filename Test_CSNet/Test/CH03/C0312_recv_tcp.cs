using System;               // Console, Int32, ArgumentException
using System.Net;           // IPAddress
using System.Net.Sockets;   // TcpListener, TcpClient

namespace Test_CSNet {
    class C0312_recv_tcp : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length != 1)   // 파라미터의 개수 확인
                throw new ArgumentException("Parameters: <Port>");

            int port = Int32.Parse(args[0]);

            // 클라이언트의 연결 요청을 수락할 TCPListener 객체를 생성한다.
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            TcpClient client = listener.AcceptTcpClient();      // 클라이언트 연결 요청을 수락한다.

            // 텍스트로 인코딩된 가격 책정 정보를 수신한다.
            ItemQuoteDecoder decoder = new ItemQuoteDecoderText();
            ItemQuote quote = decoder.decode(client.GetStream());
            Console.WriteLine("Received Text-Encoded Quote: ");
            Console.WriteLine(quote);

            // 가격 책정 정보에서 단가에 10센트를 추가한 후, 이진의 형태로 재전송한다.
            ItemQuoteEncoder encoder = new ItemQuoteEncoderBin();
            quote.unitPrice += 10;      // Add 10 cents to unit price
            Console.WriteLine("Sending (binary)...");
            byte[] bytesToSend = encoder.encode(quote);
            client.GetStream().Write(bytesToSend, 0, bytesToSend.Length);

            client.Close();
            listener.Stop();
        }
    }
}
