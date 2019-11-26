using System;                       // String, Int32, Activator
using System.Net;                   // IPAddress
using System.Net.Sockets;           // TcpListener

namespace Test_CSNet {
    class C0416_thread_main : TestObject {
        public override void OnTest(string[] args) {
            if (args.Length != 3) // 파라미터의 개수를 확인한다.
                throw new ArgumentException("Parameter(s): [<Optional properties>]" +
                    " <Port> <Protocol> <Dispatcher>");
            int servPort = Int32.Parse(args[0]);        // 서버포트
            String protocolName = args[1];              // 프로토콜 이름
            String dispatcherName = args[2];            // 전달자(dispatcher) 이름

            TcpListener listener = new TcpListener(IPAddress.Any, servPort);
            listener.Start();

            ILogger logger = new ConsoleLogger();       // 메시지를 콘솔에 기록한다.

            System.Runtime.Remoting.ObjectHandle objHandle =
                Activator.CreateInstance(null, protocolName + "ProtocolFactory");
            IProtocolFactory protoFactory = (IProtocolFactory)objHandle.Unwrap();

            objHandle = Activator.CreateInstance(null, dispatcherName + "Dispatcher");
            IDispatcher dispatcher = (IDispatcher)objHandle.Unwrap();

            dispatcher.startDispatching(listener, logger, protoFactory);
            // 이 곳으로는 도달하지 않는다.
        }
    }
}
