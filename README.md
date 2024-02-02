# Threads

A light weight library to dispatch actions to the Unity3D main-thread.
Threads can be used at both Editor-time and Run-time straight out of the box.
No initialization is required.
**When `playmode` is toggled in the editor, the actions in the queue will be purged, and thus will never complete!**
The dispatcher synchronizes with Unity during the [Update loop](https://docs.unity3d.com/Manual/ExecutionOrder.html)
or [EditorApplication.Update](https://docs.unity3d.com/ScriptReference/EditorApplication-update.html) loop.

## API

The dispatcher API can be accessed through the static [Dispatcher](Runtime/Dispatcher.cs) class.

| Method                                  | Description                                                                                                                      |
|-----------------------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| void Dispatcher.Post                    | Post an action on the main-thread                                                                                                |
| Task Dispatcher.PostAsync               | Post an awaitable action on the main-thread                                                                                      |
| IDispatchResult Dispatcher.PostAsync<T> | Post an awaitable Func<T> on the main-thread. Results can be retrieved through the [IDispatchResult](Runtime/IDispatchResult.cs) |
| Dispatcher.ToMainThread()               | An awaiter to synchronize to the main-tread                                                                                      |

See the [Docs](Documentation~) for more information or [Tests](Tests) for how to use the Dispatcher and we have provides some [Samples](Samples~/Samples.cs).

### Synchronize to the main-thread
A very useful utility in this Dispatcher is the fact that code is easily synchronized to the main-thread.
See the example below:

```csharp
Task.Run(async () => {
    // We are on some other thread depending on scheduling.
    await Dispatcher.ToMainThread();
    // We are back on the main-thread now
    // so we can access UnityEngine code 
    // without getting an exception! 
    var myGameObject = Object.Instantiate(myPrefab);
    // F*cking Awesome XD!!!
    
    // We can also await a background thread
    await Dispatcher.ToBackgroundThread();
    // We are on a bachground thread again
    // so we can do expensive processing
    // without blocking the main-thread
});
```