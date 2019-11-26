

using System;                   // Console, IAsyncResult, ArgumentException
using System.Net;               // IPEndPoint
using System.Net.Sockets;       // Socket
using System.Threading;         // ManualResetEvent
using Test_CSNet_Server;        

namespace Test_CSNet_Server {
    class ClientState {
        // 전송/수신 버퍼를 포함하는 클라이언트
        // 상태를 저장한 객체
        private const int BUFSIZE = 32; // 수신 버퍼 크기
        private byte[] rcvBuffer;
        private Socket clntSock;

        public ClientState(Socket clntSock) {
            this.clntSock = clntSock;
            rcvBuffer = new byte[BUFSIZE];      // 수신 버퍼
        }
        public byte[] RcvBuffer {
            get {
                return rcvBuffer;
            }
        }
        public Socket ClntSock {
            get {
                return clntSock;
            }
        }
    }
}

namespace Test_CSNet {
    
    class C0418_tcp_echo_server_async : TestObject {
        private const int BACKLOG = 5;      // 연결 큐의 최대 크기

        public override void OnTest(string[] args) {
            if (args.Length != 1)   // Test for correct # of args
                throw new ArgumentException("Parameters: <Port>");
            int servPort = Int32.Parse(args[0]);

            // 클라이언트의 연결 요청을 수락할 소켓을 생성한다.
            Socket servSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            servSock.Bind(new IPEndPoint(IPAddress.Any, servPort));
            servSock.Listen(BACKLOG);

            for (; ; ) {        // 무한히 실행하면서 연결 요청을 수락하고 처리한다.
                Console.WriteLine("Thread {0} ({1}) - Main(): calling BeginAccept()",
                    Thread.CurrentThread.GetHashCode(),
                    Thread.CurrentThread.ThreadState);
                IAsyncResult result = servSock.BeginAccept(new AsyncCallback(AcceptCallback), servSock);
                doOtherStuff();

                // 새로운 BeginAccept 객체를 생성하기 전에 EndAccept 객체를 대기한다.
                result.AsyncWaitHandle.WaitOne();
            }
        }

        public static void doOtherStuff() {
            for (int x = 1; x <= 5; x++) {
                Console.WriteLine("Thread {0} ({1}) - doOtherStuff(): {2}...",
                    Thread.CurrentThread.GetHashCode(),
                    Thread.CurrentThread.ThreadState, x);
                Thread.Sleep(1000);
            }
        }

        public static void AcceptCallback(IAsyncResult asyncResult) {
            Socket servSock = (Socket)asyncResult.AsyncState;
            Socket clntSock = null;

            try {
                clntSock = servSock.EndAccept(asyncResult);
                Console.WriteLine("Thread {0} ({1}) - AcceptCallback(): handling client at {2}",
                    Thread.CurrentThread.GetHashCode(),
                    Thread.CurrentThread.ThreadState, clntSock.RemoteEndPoint);
                ClientState cs = new ClientState(clntSock);
                clntSock.BeginReceive(cs.RcvBuffer, 0, cs.RcvBuffer.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveCallback), cs);
            }
            catch (SocketException se) {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                clntSock.Close();
            }
        }
        public static void ReceiveCallback(IAsyncResult asyncResult) {
            ClientState cs = (ClientState)asyncResult.AsyncState;
            try {
                int recvMsgSize = cs.ClntSock.EndReceive(asyncResult);
                if (recvMsgSize > 0) {
                    Console.WriteLine("Thread {0} ({1}) - ReceiveCallback(): received {2} bytes",
                        Thread.CurrentThread.GetHashCode(),
                        Thread.CurrentThread.ThreadState, recvMsgSize);
                    cs.ClntSock.BeginSend(cs.RcvBuffer, 0, recvMsgSize, SocketFlags.None,
                        new AsyncCallback(SendCallback), cs);
                }
                else {
                    cs.ClntSock.Close();
                }
            }
            catch (SocketException se) {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                cs.ClntSock.Close();
            }
        }
        public static void SendCallback(IAsyncResult asyncResult) {
            ClientState cs = (ClientState)asyncResult.AsyncState;
            try {
                int bytesSent = cs.ClntSock.EndSend(asyncResult);
                Console.WriteLine("Thread {0} ({1}) - SendCallback(): sent {2} bytes",
                    Thread.CurrentThread.GetHashCode(),
                    Thread.CurrentThread.ThreadState, bytesSent);
                cs.ClntSock.BeginReceive(cs.RcvBuffer, 0, cs.RcvBuffer.Length,
                    SocketFlags.None, new AsyncCallback(ReceiveCallback), cs);
            }
            catch (SocketException se) {
                Console.WriteLine(se.ErrorCode + ": " + se.Message);
                cs.ClntSock.Close();
            }
        }
    }
}
