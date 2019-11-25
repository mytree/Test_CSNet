using System.IO;            // Stream

namespace Test_CSNet {
    public interface ItemQuoteDecoder {
        ItemQuote decode(Stream source);
        ItemQuote decode(byte[] packet);
    }
}
