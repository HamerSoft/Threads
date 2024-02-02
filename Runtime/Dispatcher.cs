using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace HamerSoft.Threads
{
    /// <summary>
    /// Main-thread dispatcher which can be used to post actions on the mainthread both Editor-time and Run-time
    /// </summary>
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
            _runner?.Stop();
            _runner = null;
        }

        /// <summary>
        /// Post an action on the main-thread
        /// </summary>
        /// <param name="action">The function to be excecuted on the main-thread</param>
        public static void Post(Action action)
        {
            _runner.Post(action);
        }

        /// <summary>
        /// Post an action on the main-thread, and allow for waiting for its completion
        /// </summary>
        /// <param name="action">The function to be excecuted on the main-thread</param>
        /// <returns>An awaitable task</returns>
        public static Task PostAsync(Action action)
        {
            return _runner.PostAsync(action);
        }

        /// <summary>
        /// Post a function on the main-thread, and allow for waiting for its completion
        /// </summary>
        /// <param name="function">The function to be excecuted on the main-thread</param>
        /// <typeparam name="TResult">Result type of the function</typeparam>
        /// <returns><see cref="IDispatchResult{TResult}"/> that contains the result of the function, or the exception thrown when it was not successfull</returns>
        public static Task<IDispatchResult<TResult>> PostAsync<TResult>(Func<TResult> function)
        {
            return _runner.PostAsync(function);
        }

        /// <summary>
        /// Synchronize a thread to the main-thread
        /// </summary>
        /// <returns>an awaiter to synchronize to the main thread</returns>
        public static MainThreadSync ToMainThread()
        {
            return _runner.ToMainThread();
        }

        /// <summary>
        /// Synchronize a thread to some background-thread
        /// <remarks>This will mostly be used to sync from the main-thread to some background thread for further processing that doesn't need UnityEngine access.</remarks>
        /// </summary>
        /// <returns>an awaiter to synchronize to some background thread</returns>
        public static BackgroundThreadSync ToBackgroundThread()
        {
            return _runner.ToBackgroundThread();
        }
    }
}