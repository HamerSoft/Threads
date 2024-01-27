using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace HamerSoft.Threads
{
    public class Samples : MonoBehaviour
    {
        private Thread _thread;

        public class ConfigurableGameObject : MonoBehaviour
        {
            public void Configure(object someDto)
            {
                // Configure the GameObject
            }
        }

        /// <summary>
        /// Expensive function to load json data.
        /// Then pass it into a newly created GameObject for configuration.
        /// Being on the main-thread will be required for setting up the GameObject.
        /// </summary>
        private void ExpensiveLoadAction()
        {
            Task.Run(() =>
            {
                // This may or may not be a different thread, depending on scheduling
                var data = File.ReadAllText("my/file/path.json");
                // Now deserialize the JSON with some JSON library that supports multiple threads e.g. JsonUtility
                var dto = JsonUtility.FromJson(data, typeof(object));
                // Next we need to configure our GameObject, and we must be on the main-thread to do it.
                Dispatcher.Post(() =>
                {
                    // Now we are on the main-thread
                    var myGameObject = new GameObject().AddComponent<ConfigurableGameObject>();
                    // Pass the DTO to the configurable GameObject
                    myGameObject.Configure(dto);
                    Debug.Log("GameObject instantiated!");
                });
            });
        }

        /// <summary>
        /// Expensive function to load json data.
        /// Then pass it into a newly created GameObject for configuration.
        /// Being on the main-thread will be required for setting up the GameObject.
        ///
        /// This version uses the async variations of the Dispatcher
        /// </summary>
        private void ExpensiveLoadActionAsync()
        {
            Task.Run(async () =>
            {
                // This may or may not be a different thread, depending on scheduling
                var data = await File.ReadAllTextAsync("my/file/path.json");
                // Now deserialize the JSON with some JSON library that supports multiple threads e.g. JsonUtility
                var dto = JsonUtility.FromJson(data, typeof(object));
                // Next we need to configure our GameObject, and we must be on the main-thread to do it.
                await Dispatcher.PostAsync(() =>
                {
                    // Now we are on the main-thread
                    var myGameObject = new GameObject().AddComponent<ConfigurableGameObject>();
                    // Pass the DTO to the configurable GameObject
                    myGameObject.Configure(dto);
                });
            });
        }

        /// <summary>
        /// Expensive function to load json data.
        /// Then pass it into a newly created GameObject for configuration.
        /// Being on the main-thread will be required for setting up the GameObject.
        /// We then get the result back from the dispatcher for further processing.
        /// </summary>
        private void ExpensiveLoadActionFunction()
        {
            Task.Run(async () =>
            {
                // This may or may not be a different thread, depending on scheduling
                var data = await File.ReadAllTextAsync("my/file/path.json");
                // Now deserialize the JSON with some JSON library that supports multiple threads e.g. JsonUtility
                var dto = JsonUtility.FromJson(data, typeof(object));
                // Next we need to configure our GameObject, and we must be on the main-thread to do it.
                var dispatchResult = await Dispatcher.PostAsync(() =>
                {
                    // Now we are on the main-thread
                    var myGameObject = new GameObject().AddComponent<ConfigurableGameObject>();
                    // Pass the DTO to the configurable GameObject
                    myGameObject.Configure(dto);
                    return myGameObject;
                });
                // We are back on the thread we started with now.

                // We now have a DispatchResult for further processing
                if (dispatchResult.Succeeded)
                {
                    // Do something with the result ( mind, we might not be on the main-thread!)
                    var myResult = dispatchResult.Result;
                }
                else
                {
                    // Dispatch did not succeed, thus let's handle the exception
                    Debug.LogException(dispatchResult.Exception);
                }
            });
        }

        /// <summary>
        /// Expensive function to load json data.
        /// Then pass it into a newly created GameObject for configuration.
        /// Being on the main-thread will be required for setting up the GameObject.
        /// We will synchronize with the main thread and continue processing the object there.
        /// </summary>
        private void ExpensiveLoadActionAwaitMainThread()
        {
            Task.Run(async () =>
            {
                // This may or may not be a different thread, depending on scheduling
                var data = await File.ReadAllTextAsync("my/file/path.json");
                // Now deserialize the JSON with some JSON library that supports multiple threads e.g. JsonUtility
                var dto = JsonUtility.FromJson(data, typeof(object));
                // Next we need to configure our GameObject, and we must be on the main-thread to do it.
                await Dispatcher.ToMainThread();

                // Now we are on the main-thread!

                var myGameObject = new GameObject().AddComponent<ConfigurableGameObject>();
                // Pass the DTO to the configurable GameObject
                myGameObject.Configure(dto);

                // Let's synchronize to some background thread again to do some further processing
                await Dispatcher.ToBackgroundThread();
                // We are on a background thread now.
                // Thus we will not block the main-thread with expensive operations.
            });
        }

        /// <summary>
        /// Expensive function to load json data.
        /// Then pass it into a newly created GameObject for configuration.
        /// Being on the main-thread will be required for setting up the GameObject.
        /// We will synchronize with the main thread and continue processing the object there.
        /// </summary>
        private void ThreadExample()
        {
            _thread = new Thread(
                async () =>
                {
                    // This may or may not be a different thread, depending on scheduling
                    var data = await File.ReadAllTextAsync("my/file/path.json");
                    // Now deserialize the JSON with some JSON library that supports multiple threads e.g. JsonUtility
                    var dto = JsonUtility.FromJson(data, typeof(object));
                    // Next we need to configure our GameObject, and we must be on the main-thread to do it.
                    await Dispatcher.ToMainThread();

                    // Now we are on the main-thread!

                    var myGameObject = new GameObject().AddComponent<ConfigurableGameObject>();
                    // Pass the DTO to the configurable GameObject
                    myGameObject.Configure(dto);

                    // Let's synchronize to some background thread again to do some further processing
                    await Dispatcher.ToBackgroundThread();
                    // We are on a background thread now.
                    // Thus we will not block the main-thread with expensive operations.
                });
            _thread.Start();
        }

        private void OnDestroy()
        {
            _thread?.Abort();
            _thread = null;
        }
    }
}