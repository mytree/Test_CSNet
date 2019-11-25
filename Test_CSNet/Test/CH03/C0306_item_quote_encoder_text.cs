using System;
using System.IO;
using System.Text;

namespace Test_CSNet {
    public class ItemQuoteEncoderText : ItemQuoteEncoder {
        public Encoding encoding;           // 캐릭터 인코딩
        public ItemQuoteEncoderText()
            : this(ItemQuoteTextConst.DEFAULT_CHAR_ENC) {
        }
        public ItemQuoteEncoderText(string encodingDesc) {
            encoding = Encoding.GetEncoding(encodingDesc);
        }
        public byte[] encode(ItemQuote item) {
            String EncodedString = item.itemNumber + " ";
            if (item.itemDescription.IndexOf('\n') != -1)
                throw new IOException("Invalid description (contains newline)");
            EncodedString = EncodedString + item.itemDescription + "\n";
            EncodedString = EncodedString + item.quantity + " ";
            EncodedString = EncodedString + item.unitPrice + " ";

            if (item.discounted)
                EncodedString = EncodedString + "d";        // 할인 적용 시 'd' 추가
            if (item.inStock)
                EncodedString = EncodedString + "s";        // 재고 있을 경우 's' 추가

            if (EncodedString.Length > ItemQuoteTextConst.MAX_WIRE_LENGTH)
                throw new IOException("Encoded length too long");
            byte[] buf = encoding.GetBytes(EncodedString);
            return buf;
        }
    }
}
