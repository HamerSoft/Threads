namespace HamerSoft.Threads
{
    /// <summary>
    /// Object that facilitates synchronization to the main-thread
    /// </summary>
    public class MainThreadSync
    {
        private readonly MainThread _mainThread;

        internal MainThreadSync(MainThread mainThread)
        {
            _mainThread = mainThread;
        }

        /// <summary>
        /// Get the awaiter
        /// </summary>
        /// <returns>Main-thread Awaiter</returns>
        public MainThreadAwaiter GetAwaiter()
        {
            var awaiter = new MainThreadAwaiter(_mainThread);
            _mainThread.Run(() => awaiter.Complete(null));
            return awaiter;
        }
    }
}