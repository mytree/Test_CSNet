using System;                       // String
using System.Net;                   // IPEndPoint, EndPoint
using System.Net.Sockets;           // TcpClient, SocketShutdown

namespace Test_CSNet {
    class TcpClientShutdown : TcpClient {
        public TcpClientShutdown() : base() { }
        public TcpClientShutdown(IPEndPoint localEP) : base(localEP) { }
        public TcpClientShutdown(String server, int port) : base(server, port) { }
        public void Shutdown(SocketShutdown ss) {
            // 내부 소켓에 대한 Shutdown 메소드를 호출한다.
            this.Client.Shutdown(ss);
        }
        public EndPoint GetRemoteEndPoint() {
            // 내부 소켓으로부터 RemoteEndPoint 객체를 반환한다.
            return this.Client.RemoteEndPoint;
        }
    }
}
