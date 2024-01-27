using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace HamerSoft.Threads
{
    internal class Runner
    {
        private IUpdater _updater;
        private MainThread _mainThread;
        private ConcurrentQueue<Action> _actionQueue;

        internal Runner(IUpdater updater)
        {
            _mainThread = new MainThread();
            _actionQueue = new ConcurrentQueue<Action>();
            _updater = updater;
            _updater.Stopped += OnStopped;
            _updater.Updated += OnUpdated;
        }

        internal void Post(Action function)
        {
            _actionQueue.Enqueue(function);
        }

        internal Task PostAsync(Action function)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            void SafeAction()
            {
                try
                {
                    function();
                    taskCompletionSource.TrySetResult(true);
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetException(e);
                }
            }

            _actionQueue.Enqueue(SafeAction);
            return taskCompletionSource.Task;
        }

        internal Task<IDispatchResult<TResult>> PostAsync<TResult>(Func<TResult> function)
        {
            var taskCompletionSource = new TaskCompletionSource<IDispatchResult<TResult>>();

            void SafeAction()
            {
                try
                {
                    var result = function();
                    taskCompletionSource.TrySetResult(new DispatchResult<TResult>(result));
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetResult(new DispatchResult<TResult>(e));
                }
            }

            _actionQueue.Enqueue(SafeAction);
            return taskCompletionSource.Task;
        }

        internal MainThreadSync ToMainThread()
        {
            return new MainThreadSync(_mainThread);
        }

        internal BackgroundThreadSync ToBackgroundThread()
        {
            return new BackgroundThreadSync();
        }
        
        private void OnUpdated()
        {
            while (_actionQueue.TryDequeue(out var action))
                action?.Invoke();
        }

        private void OnStopped()
        {
            _updater.Stopped -= OnStopped;
            _updater.Updated -= OnUpdated;
            Stop();
        }

        internal void Stop()
        {
            _mainThread = null;
            _actionQueue.Clear();
            _updater = null;
        }
    }
}