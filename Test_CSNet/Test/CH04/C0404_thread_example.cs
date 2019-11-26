using System;               // String
using System.Threading;     // Thread

namespace Test_CSNet {
    class MyThreadClass {
        // String 인사를 입력 값으로 받아들이고, 이 값을 자신의 스레드에서 무작위의 주기로
        // 콘솔에 10번 출력한다.
        private const int RANDOM_SLEEP_MAX = 500;       // 무작위로 대기할 경우의 최대 값
        private const int LOOP_COUNT = 10;              // 메시지를 출력할 횟수
        private String greeting;                        // 콘솔에 출력할 메시지
        public MyThreadClass(String greeting) {
            this.greeting = greeting;
        }
        public void runMyThread() {
            Random rand = new Random();
            for (int x = 0; x < LOOP_COUNT; x++) {
                Console.WriteLine("Thread-" + Thread.CurrentThread.GetHashCode() + ": " + greeting);
                try {
                    // 0 에서 RANDOM_SLEEP_MAX 밀리초 만큼 대기한다.
                    Thread.Sleep(rand.Next(RANDOM_SLEEP_MAX));
                }
                catch (ThreadInterruptedException) { }  // 실행되지 않는다.
            }
        }
    }
    class C0404_thread_example : TestObject {
        public override void OnTest(string[] args) {
            MyThreadClass mtc1 = new MyThreadClass("Hello");
            new Thread(new ThreadStart(mtc1.runMyThread)).Start();

            MyThreadClass mtc2 = new MyThreadClass("Aloha");
            new Thread(new ThreadStart(mtc2.runMyThread)).Start();

            MyThreadClass mtc3 = new MyThreadClass("Ciao");
            new Thread(new ThreadStart(mtc2.runMyThread)).Start();
        }
    }
}
