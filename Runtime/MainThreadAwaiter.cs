using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace HamerSoft.Threads
{
    /// <summary>
    /// An awaitable to synchronize to the main thread
    /// </summary>
    public class MainThreadAwaiter : INotifyCompletion
    {
        private readonly MainThread _mainThread;
        private bool _isDone;
        private Exception _exception;
        private Action _continuation;

        /// <summary>
        /// Flag if the awaitable is complete
        /// </summary>
        public bool IsCompleted
        {
            get { return _isDone; }
        }

        /// <summary>
        /// Get the result of the awaitable
        /// </summary>
        public void GetResult()
        {
            if (_exception != null)
            {
                ExceptionDispatchInfo.Capture(_exception).Throw();
            }
        }

        internal MainThreadAwaiter(MainThread mainThread)
        {
            _mainThread = mainThread;
        }

        /// <summary>
        /// Complete the awaitable
        /// </summary>
        /// <param name="e">optional exception</param>
        public void Complete(Exception e)
        {
            _isDone = true;
            _exception = e;

            if (_continuation != null)
            {
                _mainThread.Run(_continuation);
            }
        }

        /// <summary>
        /// Function invoked when completed
        /// </summary>
        /// <param name="continuation">continuation action</param>
        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }
    }
}