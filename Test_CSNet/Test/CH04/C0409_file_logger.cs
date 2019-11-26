using System;                       // String
using System.IO;                    // StreamWriter
using System.Threading;             // Mutex
using System.Collections;           // ArrayList

namespace Test_CSNet {
    class FileLogger : ILogger {
        private static Mutex mutex = new Mutex();
        private StreamWriter output;        // 로그 파일
        public FileLogger(String filename) {
            // 로그 파일을 생성한다.
            output = new StreamWriter(filename, true);
        }
        public void writeEntry(ArrayList entry) {
            mutex.WaitOne();
            IEnumerator line = entry.GetEnumerator();
            while (line.MoveNext())
                output.WriteLine(line.Current);
            output.WriteLine();
            output.Flush();
            mutex.ReleaseMutex();
        }

        public void writeEntry(string entry) {
            mutex.WaitOne();
            output.WriteLine(entry);
            output.WriteLine();
            output.Flush();
            mutex.ReleaseMutex();
        }
    }
}
