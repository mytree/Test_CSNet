using System;                       // String, Int32, Console
using System.Text;                  // Encoding
using System.Net;                   // IPAddress
using System.Net.Sockets;           // TcpListener, TcpClient, NetworkStream

namespace Test_CSNet {
    class C0424_transcode_server : TestObject {
        public static readonly int BUFSIZE = 1024;      // 읽기 버퍼의 크기
        public override void OnTest(string[] args) {
            if (args.Length != 1)       // 파라미터 개수 확인
                throw new ArgumentException("Parameter(s): <Port>");
            int servPort = Int32.Parse(args[0]);        // 서버 포트
            // TcpListener 인스턴스를 생성하고 클라이언트 연결 요청을 수락한다.
            TcpListener listener = new TcpListener(IPAddress.Any, servPort);
            listener.Start();

            byte[] buffer = new byte[BUFSIZE];      // 읽기/쓰기 버퍼를 할당한다.
            int bytesRead;                          // 읽어들인 바이트 수
            for (; ; ) {        // 무한히 실행하면서 연결 요청을 수락하고 처리한다.
                // 클라이언트가 연결하기를 대기한 다음 새 TcpClient 인스턴스를 생성한다.
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("\nHandling client...");

                // 소켓으로부터 입출력 스트림을 가져온다.
                NetworkStream netStream = client.GetStream();

                int totalBytesRead = 0;
                int totalBytesWritten = 0;
                Decoder uniDecoder = Encoding.Unicode.GetDecoder();
                Char[] chars = null;

                // 클라이언트가 0의 값을 전송하여 연결을 종료할 때까지 계속 수신한다.
                while ((bytesRead = netStream.Read(buffer, 0, buffer.Length)) > 0) {
                    totalBytesRead += bytesRead;

                    // 입력받은 바이트를 유니코드 Char 어레이로 변환한다.
                    int charCount = uniDecoder.GetCharCount(buffer, 0, bytesRead);
                    chars = new Char[charCount];
                    int charsDecodedCount = uniDecoder.GetChars(buffer, 0, bytesRead, chars, 0);

                    // 유니코드 char 어레이를 UTF8 바이트로 변환한다.
                    int byteCount = Encoding.UTF8.GetByteCount(chars, 0, charsDecodedCount);
                    byte[] outputBuffer = new byte[byteCount];
                    Encoding.UTF8.GetBytes(chars, 0, charsDecodedCount, outputBuffer, 0);

                    // UTF-8 바이트를 클라이언트로 재전송한다.
                    netStream.Write(outputBuffer, 0, outputBuffer.Length);
                    totalBytesWritten += outputBuffer.Length;
                }

                Console.WriteLine("Total bytes read:    {0}", totalBytesRead);
                Console.WriteLine("Total bytes written:      {0} ", totalBytesWritten);
                Console.WriteLine("Closing client connection...");

                netStream.Close();      // 스트림을 종료한다.
                client.Close();         // 소켓을 종료한다.
            }
        }
    }
}
