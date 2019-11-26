using System;                   // String
using System.Collections;       // ArrayList

namespace Test_CSNet {
    public interface ILogger {
        void writeEntry(ArrayList entry);       // 목록 전체를 기록한다.
        void writeEntry(String entry);          // 한 줄만 기록한다.
    }
}
