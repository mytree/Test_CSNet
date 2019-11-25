using System;           // String, Activator
using System.IO;        // Stream
using System.Text;      // Encoding
using System.Net;       // IPAddress

namespace Test_CSNet {
    public class ItemQuoteDecoderBin : ItemQuoteDecoder {
        public Encoding encoding;       // 캐릭터 인코딩
        public ItemQuoteDecoderBin()
            : this(ItemQuoteTextConst.DEFAULT_CHAR_ENC) {
        }
        public ItemQuoteDecoderBin(String encodingDesc) {
            encoding = Encoding.GetEncoding(encodingDesc);
        }
        public ItemQuote decode(Stream wire) {
            BinaryReader src = new BinaryReader(new BufferedStream(wire));

            long itemNumber = IPAddress.NetworkToHostOrder(src.ReadInt64());
            int quantity = IPAddress.NetworkToHostOrder(src.ReadInt32());
            int unitPrice = IPAddress.NetworkToHostOrder(src.ReadInt32());
            byte flags = src.ReadByte();

            int stringLength = src.Read();      // 정수 형태의 언사인드 비트를 반환한다.
            if (stringLength == -1)
                throw new EndOfStreamException();
            byte[] stringBuf = new byte[stringLength];
            src.Read(stringBuf, 0, stringLength);
            String itemDesc = encoding.GetString(stringBuf);

            return new ItemQuote(itemNumber, itemDesc, quantity, unitPrice, 
                ((flags & ItemQuoteBinConst.DISCOUNT_FLAG) == ItemQuoteBinConst.DISCOUNT_FLAG),
                ((flags & ItemQuoteBinConst.IN_STOCK_FLAG) == ItemQuoteBinConst.IN_STOCK_FLAG));
        }
        public ItemQuote decode(byte[] packet) {
            Stream payload = new MemoryStream(packet, 0, packet.Length, false);
            return decode(payload);
        }
    }
}
