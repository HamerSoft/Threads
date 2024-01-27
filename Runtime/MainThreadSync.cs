namespace Hamersoft.Threads
{
    public class MainThreadSync
    {
        private readonly MainThread _mainThread;

        internal MainThreadSync(MainThread mainThread)
        {
            _mainThread = mainThread;
        }

        public MainThreadAwaiter GetAwaiter()
        {
            var awaiter = new MainThreadAwaiter(_mainThread);
            _mainThread.Run(() => awaiter.Complete(null));
            return awaiter;
        }
    }
}