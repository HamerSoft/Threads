using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Hamersoft.Threads
{
    public static class Dispatcher
    {
        private static IUpdater _update;
        private static Runner _runner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Start()
        {
            if (_update != null)
                return;
            _update = new GameObject("Update").AddComponent<UpdateLoop>();
            _runner = new Runner(_update);
        }

        internal static void Start(IUpdater updater)
        {
            _update = updater;
            _runner?.Stop();
            _runner = new Runner(updater);
        }

        internal static void Stop()
        {
            _update = null;
            _runner.Stop();
            _runner = null;
        }

        public static void Post(Action action)
        {
            _runner.Post(action);
        }
        
        public static Task<IDispatchResult<TResult, Exception>> PostAsync<TResult>(Func<TResult> function)
        {
            return _runner.PostAsync(function);
        }

        public static MainThreadSync ToMainThread()
        {
            return _runner.ToMainThread();
        }
    }
}