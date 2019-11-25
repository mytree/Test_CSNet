using System;               // Boolean
using System.IO;            // Stream 

namespace Test_CSNet {
    class Framer {
        public static byte[] nextToken(Stream input, byte[] delimiter) {
            int nextByte;
            // 스트림이 이미 종료되었다면, null 을 반환한다.
            if ((nextByte = input.ReadByte()) == -1)
                return null;

            MemoryStream tokenBuffer = new MemoryStream();
            do {
                tokenBuffer.WriteByte((byte)nextByte);
                byte[] currentToken = tokenBuffer.ToArray();
                if (endsWith(currentToken, delimiter)) {
                    int tokenLength = currentToken.Length - delimiter.Length;
                    byte[] token = new byte[tokenLength];
                    Array.Copy(currentToken, 0, token, 0, tokenLength);
                    return token;
                }
            } while ((nextByte = input.ReadByte()) != -1);  // EOS 일 경우 중지한다.
            return tokenBuffer.ToArray();           // 최소한 한 개 바이트를 수신
        }

        // suffix 가 나타내는 바이트 조합으로 데이터가 끝날 경우, true 를 반환한다.
        private static Boolean endsWith(byte[] value, byte[] suffix) {
            if (value.Length < suffix.Length)
                return false;

            for (int offset = 1; offset <= suffix.Length; offset++) {
                if (value[value.Length - offset] != suffix[suffix.Length - offset])
                    return false;
            }
            return true;
        }
    }
}
