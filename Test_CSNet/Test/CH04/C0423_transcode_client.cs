using System;                   // String, Int32, ArgumentException
using System.IO;                // FileStream
using System.Net.Sockets;       // NetworkStream, TcpClient

namespace Test_CSNet {
    class C0423_transcode_client : TestObject {
        private const int BUFSIZE = 256;        // 읽기 버퍼의 크기
        private static NetworkStream netStream;
        private static FileStream fileIn;
        private static TcpClientShutdown client;
        public override void OnTest(string[] args) {
            if (args.Length != 3) // 파라미터의 개수
                throw new ArgumentException("Parameter(s): <Server> <Port> <File>");
            String server = args[0];        // 서버명 혹은 IP 주소
            int port = Int32.Parse(args[1]);    // 서버 포트
            String filename = args[2];      // 데이터를 읽어들일 파일명
            // 입력 및 출력에 사용할 파일을 연다.(이름: <input>.ut8)
            fileIn = new FileStream(filename, FileMode.Open, FileAccess.Read);
            FileStream fileOut = new FileStream(filename + ".utf8", FileMode.Create);

            // TcpClient 인스턴스를 생성하여 서버의 해당 포트에 결합한다.
            client = new TcpClientShutdown();
            client.Connect(server, port);

            // 인코딩되지 않은 바이트 스트림을 서버로 전송한다.
            netStream = client.GetStream();
            sendBytes();

            // 서버로부터 인코딩된 바이트 스트림을 수신한다.
            int bytesRead;      // 읽어들인 바이트 수
            byte[] buffer = new byte[BUFSIZE];      // 바이트 버퍼
            while ((bytesRead = netStream.Read(buffer, 0, buffer.Length)) > 0) {
                fileOut.Write(buffer, 0, bytesRead);
                Console.Write("R");     // 읽기 작업 완료를 표시한다.
            }
            Console.WriteLine();        // 쓰기 작업 완료를 표시한다.
            netStream.Close();      // 스트림을 종료한다.
            client.Close();     // 소켓을 종료한다.
            fileIn.Close();     // 입력파일을 종료한다.
            fileOut.Close();        // 출력파일을 종료한다.
        }

        private static void sendBytes() {
            int bytesRead;      // 읽어들인 바이트 수
            BufferedStream fileInBuf = new BufferedStream(fileIn);
            byte[] buffer = new byte[BUFSIZE];      // Byte buffer
            while ((bytesRead = fileInBuf.Read(buffer, 0, buffer.Length)) > 0) {
                netStream.Write(buffer, 0, bytesRead);
                Console.Write("W");     // 쓰기 작업 완료를 표시한다.
            }
            client.Shutdown(SocketShutdown.Send);       // 전송 완료
        }
    }
}
