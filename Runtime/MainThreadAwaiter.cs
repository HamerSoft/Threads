using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Hamersoft.Threads
{
    public class MainThreadAwaiter : INotifyCompletion
    {
        private readonly MainThread _mainThread;
        private bool _isDone;
        private Exception _exception;
        private Action _continuation;

        public bool IsCompleted
        {
            get { return _isDone; }
        }

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

        public void Complete(Exception e)
        {
            _isDone = true;
            _exception = e;

            if (_continuation != null)
            {
                _mainThread.Run(_continuation);
            }
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }
    }
}