using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Hamersoft.Threads
{
    internal class Runner
    {
        private IUpdater _updater;
        private MainThread _mainThread;
        private ConcurrentQueue<Action> _actionQueue;

        public Runner(IUpdater updater)
        {
            _mainThread = new MainThread();
            _actionQueue = new ConcurrentQueue<Action>();
            _updater = updater;
            _updater.Stopped += OnStopped;
            _updater.Updated += OnUpdated;
        }

        public void Post(Action function)
        {
            _actionQueue.Enqueue(function);
        }

        public Task PostAsync(Action function)
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

        public Task<IDispatchResult<TResult, Exception>> PostAsync<TResult>(Func<TResult> function)
        {
            var taskCompletionSource = new TaskCompletionSource<IDispatchResult<TResult, Exception>>();

            void SafeAction()
            {
                try
                {
                    var result = function();
                    taskCompletionSource.TrySetResult(new DispatchResult<TResult, Exception>(result));
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetResult(new DispatchResult<TResult, Exception>(e));
                }
            }

            _actionQueue.Enqueue(SafeAction);
            return taskCompletionSource.Task;
        }

        public MainThreadSync ToMainThread()
        {
            return new MainThreadSync(_mainThread);
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

        public void Stop()
        {
            _mainThread = null;
            _actionQueue.Clear();
            _updater = null;
        }
    }
}