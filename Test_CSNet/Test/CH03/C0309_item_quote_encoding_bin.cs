using System;           // String, Activator
using System.IO;        // BinaryWriter
using System.Text;      // Encoding
using System.Net;       // IPAddress

namespace Test_CSNet {
    public class ItemQuoteEncoderBin : ItemQuoteEncoder {
        public Encoding encoding;       // 캐릭터 인코딩
        public ItemQuoteEncoderBin()
            : this(ItemQuoteBinConst.DEFAULT_CHAR_ENC) {
        }
        public ItemQuoteEncoderBin(String encodingDesc) {
            encoding = Encoding.GetEncoding(encodingDesc);
        }
        public byte[] encode(ItemQuote item) {
            MemoryStream mem = new MemoryStream();
            BinaryWriter output = new BinaryWriter(new BufferedStream(mem));
            output.Write(IPAddress.HostToNetworkOrder(item.itemNumber));
            output.Write(IPAddress.HostToNetworkOrder(item.quantity));
            output.Write(IPAddress.HostToNetworkOrder(item.unitPrice));

            byte flags = 0;
            if (item.discounted)
                flags |= ItemQuoteBinConst.DISCOUNT_FLAG;
            if (item.inStock)
                flags |= ItemQuoteBinConst.IN_STOCK_FLAG;
            output.Write(flags);

            byte[] encodedDesc = encoding.GetBytes(item.itemDescription);
            if (encodedDesc.Length > ItemQuoteBinConst.MAX_DESC_LEN)
                throw new IOException("Item Description exceeds encoded length limit");
            output.Write((byte)encodedDesc.Length);
            output.Write(encodedDesc);

            output.Flush();

            return mem.ToArray();
        }
    }
}
