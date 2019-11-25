using System;               // String, Console, ArgumentException
using System.Net.Sockets;   // TcpClient, NetworkStream

namespace Test_CSNet {
    class C0311_send_tcp : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length != 2)       // Test for correct # of args
                throw new ArgumentException("Parameters: <Destination> <Port>");
            String server = args[0];        // 목적지 주소
            int servPort = Int32.Parse(args[1]);        // 목적지 포트

            // 서버의 해당 포트에 접속하는 소켓 객체를 생성한다.
            TcpClient client = new TcpClient(server, servPort);
            NetworkStream netStream = client.GetStream();

            ItemQuote quote = new ItemQuote(1234567890987654L, "5mm Super Widgets",
                100, 12999, true, false);
            // 텍스트 형태로 인코딩 한 가격 책정(quote) 정보를 전송한다.
            ItemQuoteEncoderText coder = new ItemQuoteEncoderText();
            byte[] codedQuote = coder.encode(quote);
            Console.WriteLine("Sending Text-Encoded Quote (" + codedQuote.Length + "bytes): ");
            Console.Write(quote);

            netStream.Write(codedQuote, 0, codedQuote.Length);

            // 이진 형태로 인코딩한 가격 책정 정보를 수신한다.
            ItemQuoteDecoder decoder = new ItemQuoteDecoderBin();
            ItemQuote receivedQuote = decoder.decode(client.GetStream());
            Console.WriteLine("Received Binary-Encode Quote: ");
            Console.WriteLine(receivedQuote);

            netStream.Close();
            client.Close();
        }
    }
}
