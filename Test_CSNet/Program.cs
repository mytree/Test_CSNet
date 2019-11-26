using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CSNet
{
    class Program
    {
        static void Main(string[] args)
        {
            //C0201_ipaddress test = new C0201_ipaddress();
            //C0202_tcp_echo_client test = new C0202_tcp_echo_client();
            //C0203_tcp_echo_server test = new C0203_tcp_echo_server();
            //C0204_udp_echo_client test = new C0204_udp_echo_client();
            //C0205_udp_echo_server test = new C0205_udp_echo_server();

            C0404_thread_example test = new C0404_thread_example();
            test.OnTest(args);
        }
    }
}
