/*
 * [실행결과] 인자값(www.google.co.kr)
 * Local Host:
 *      Host Name: TREE-PC-01
 *      Canonical Name: TREE-PC-01
 *      IP Addresses: fe80::7419:b2dc:1247:1d8b%11 fe80::c462:93b0:73a3:38b8%6 192.168.56.1 192.168.1.41 192.168.0.41
 *      Aliases:
 *      
 * www.google.co.kr:
 *      Canonical Name: www.google.co.kr
 *      IP Addresses: 172.217.31.131
 *      Aliases:
 *      
 * 계속하려면 아무 키나 누르십시오 . . .
 * 
 */

using System;                       // String Console
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;                   // Dns, IPHostEntry, IPAddress
using System.Net.Sockets;           // SocketException

namespace Test_CSNet
{
    class C0201_ipaddress : TestObject
    {
        public override void OnTest(String[] args) {

            // 로컬 호스트 정보를 취합하고 출력한다.
            try
            {
                Console.WriteLine("Local Host: ");
                String localHostName = Dns.GetHostName();
                Console.WriteLine("\tHost Name: " + localHostName);
                PrintHostInfo(localHostName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to resolve local host\n" + ex.ToString());
            }

            // Command-Line 에서 입력한 호스트들에 대한 정보를 취합하고 출력한다.
            foreach (String arg in args)
            {
                Console.WriteLine(arg + ":");
                PrintHostInfo(arg);
            }
        }

        static void PrintHostInfo(String host)
        {
            try {
                IPHostEntry hostInfo;
                // 호스트 또는 주소에 대한 DNS 해석한다.
                //hostInfo = Dns.Resolve(host); // warning CS0618 : use GetHostEntry instead
                hostInfo = Dns.GetHostEntry(host);
                // 호스트의 기본 호스트명을 출력한다.
                Console.WriteLine("\tCanonical Name: " + hostInfo.HostName);
                // 호스트에 대한 IP 주소들을 출력한다.
                Console.Write("\tIP Addresses: ");
                foreach (IPAddress ipaddr in hostInfo.AddressList)
                {
                    Console.Write(ipaddr.ToString() + " ");
                }
                Console.WriteLine();

                // 이 호스트의 앨리어스들을 출력한다.
                Console.Write("\tAliases: ");
                foreach (String alias in hostInfo.Aliases)
                {
                    Console.Write(alias + " ");
                }
                Console.WriteLine("\n");
            }
            catch (Exception ex) {
                Console.WriteLine("\tUnable to resolve host: " + host + "\n" + ex.ToString());
            }
        }
    }
}
