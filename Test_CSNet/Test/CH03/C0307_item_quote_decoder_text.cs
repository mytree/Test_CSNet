using System;           // String, Activator
using System.Text;      // Encoding
using System.IO;        // Stream

namespace Test_CSNet {
    public class ItemQuoteDecoderText : ItemQuoteDecoder {
        public Encoding encoding;       // 캐릭터 인코딩
        public ItemQuoteDecoderText()
            : this(ItemQuoteTextConst.DEFAULT_CHAR_ENC) {
        }
        public ItemQuoteDecoderText(String encodingDesc) {
            encoding = Encoding.GetEncoding(encodingDesc);
        }
        public ItemQuote decode(Stream wire) {
            String itemNo, description, quant, price, flags;

            byte[] space = encoding.GetBytes(" ");
            byte[] newline = encoding.GetBytes("\n");
            itemNo = encoding.GetString(Framer.nextToken(wire, space));
            description = encoding.GetString(Framer.nextToken(wire, newline));
            quant = encoding.GetString(Framer.nextToken(wire, space));
            price = encoding.GetString(Framer.nextToken(wire, space));
            flags = encoding.GetString(Framer.nextToken(wire, newline));
            return new ItemQuote(Int64.Parse(itemNo), description,
                Int32.Parse(quant), Int32.Parse(price),
                (flags.IndexOf('d') != -1),
                (flags.IndexOf('s') != -1));
        }
        public ItemQuote decode(byte[] packet) {
            Stream payload = new MemoryStream(packet, 0, packet.Length, false);
            return decode(payload);
        }
    }
}
