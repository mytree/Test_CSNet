using System;           // String

namespace Test_CSNet {
    public class MCIPAddress {
        public static Boolean isValid(String ip) {
            try {
                int octet1 = Int32.Parse(ip.Split(new Char[] { '.' }, 4)[0]);
                if ((octet1 >= 224) && (octet1 <= 239)) return true;
            }
            catch (Exception) {
            }
            return false;
        }
    }
}
