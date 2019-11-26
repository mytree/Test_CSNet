using System;                       // String, IAsyncResult, ArgumentException
using System.Text;                  // Encoding
using System.Net.Sockets;           // TcpClient, NetworkStream
using System.Threading;             // ManualResetEvent

using Test_CSNet_Client;

namespace Test_CSNet_Client {
    class ClientState {
        // 네트워크 스트림 및 전송/수신 버퍼를 포함하는 클라이언트 상태를 저장한 객체
        private byte[] byteBuffer;
        private NetworkStream netStream;
        private StringBuilder echoResponse;
        private int totalBytesRcvd = 0;      // 현재까지 수신한 바이트 수

        public ClientState(NetworkStream netStream, byte[] byteBuffer) {
            this.netStream = netStream;
            this.byteBuffer = byteBuffer;
            echoResponse = new StringBuilder();
        }
        public NetworkStream NetStream {
            get {
                return netStream;
            }
        }
        public byte[] ByteBuffer {
            set {
                byteBuffer = value;
            }
            get {
                return byteBuffer;
            }
        }
        public void AppendResponse(String response) {
            echoResponse.Append(response);
        }
        public String EchoResponse {
            get {
                return echoResponse.ToString();
            }
        }
        public void AddToTotalBytes(int count) {
            totalBytesRcvd += count;
        }
        public int TotalBytes {
            get {
                return totalBytesRcvd;
            }
        }
    }
}

namespace Test_CSNet {
    
    class C0417_tcp_echo_client_async : TestObject {
        // 모든 읽기 작업이 끝나면 수동 이벤트 신호를 발생시킨다.
        public static ManualResetEvent ReadDone = new ManualResetEvent(false);

        public override void OnTest(string[] args) {
            if (args.Length < 2 || args.Length > 3) {   // 파라미터 개수 확인
                throw new ArgumentException("Parameters: <Server> <Word> [<Port>]");
            }
            String server = args[0];        // 서버명 혹은 IP 주소

            // 포트 번호가 파라미터로 입력되었는지를 확인. 아닐 경우 7로 사용한다.
            int servPort = (args.Length == 3) ? Int32.Parse(args[2]) : 7;
            Console.WriteLine("Thread {0} ({1}) - Main()",
                Thread.CurrentThread.GetHashCode(), Thread.CurrentThread.ThreadState);
            // 해당 서버 포트에 접속하는 TcpClient 객체를 생성한다.
            TcpClient client = new TcpClient();

            client.Connect(server, servPort);
            Console.WriteLine("Thread {0} ({1}) - Main(): connected to server",
                Thread.CurrentThread.GetHashCode(), Thread.CurrentThread.ThreadState);

            NetworkStream netStream = client.GetStream();
            ClientState cs = new ClientState(netStream, Encoding.ASCII.GetBytes(args[1]));
            // 인코딩 된 스트링을 서버로 전송한다.
            IAsyncResult result = netStream.BeginWrite(cs.ByteBuffer, 0,
                cs.ByteBuffer.Length, new AsyncCallback(WriteCallback), cs);
            doOtherStuff();
            result.AsyncWaitHandle.WaitOne();       // EndWrite() 메소드 호출때까지 블록
            // 서버로부터 동일한 스트링을 다시 수신한다.
            result = netStream.BeginRead(cs.ByteBuffer, cs.TotalBytes,
                cs.ByteBuffer.Length - cs.TotalBytes, new AsyncCallback(ReadCallback), cs);
            doOtherStuff();
            ReadDone.WaitOne();     // ReadDone 이 수동적으로 설정될 때까지 블록을 건다.

            netStream.Close();      // 스트림을 종료한다.
            client.Close();     // 소켓 종료
        }
        public static void doOtherStuff() {
            for (int x = 1; x <= 5; x++) {
                Console.WriteLine("Thread {0} ({1}) - doOtherStuff(): {2} ...",
                    Thread.CurrentThread.GetHashCode(), Thread.CurrentThread.ThreadState, x);
                Thread.Sleep(1000);
            }
        }
        public static void WriteCallback(IAsyncResult asyncResult) {
            ClientState cs = (ClientState)asyncResult.AsyncState;
            cs.NetStream.EndWrite(asyncResult);
            Console.WriteLine("Thread {0} ({1}) - WriteCallback(): Sent {2} bytes...",
                Thread.CurrentThread.GetHashCode(), Thread.CurrentThread.ThreadState, cs.ByteBuffer.Length);
        }
        public static void ReadCallback(IAsyncResult asyncResult) {
            ClientState cs = (ClientState)asyncResult.AsyncState;
            int bytesRcvd = cs.NetStream.EndRead(asyncResult);

            cs.AddToTotalBytes(bytesRcvd);
            cs.AppendResponse(Encoding.ASCII.GetString(cs.ByteBuffer, 0, bytesRcvd));
            if (cs.TotalBytes < cs.ByteBuffer.Length) {
                Console.WriteLine("Thread {0} ({1}) - ReadCallback(): Received {2} bytes...",
                    Thread.CurrentThread.GetHashCode(),
                    Thread.CurrentThread.ThreadState, bytesRcvd);
                cs.NetStream.BeginRead(cs.ByteBuffer, cs.TotalBytes,
                    cs.ByteBuffer.Length - cs.TotalBytes,
                    new AsyncCallback(ReadCallback), cs.NetStream);
            }
            else {
                Console.WriteLine("Thread {0} ({1}) - ReadCallback(): Received {2} total " +
                    "bytes: {3}", Thread.CurrentThread.GetHashCode(),
                    Thread.CurrentThread.ThreadState, cs.TotalBytes, cs.EchoResponse);
                ReadDone.Set();     // 읽기 작업 종료 신호를 발생시킨다.
            }
        }
    }
}
