using System;
using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace HamerSoft.Threads.Tests.Runtime
{
    [TestFixture]
    public class RuntimeDispatcherTests
    {
        private int _mainThread;

        [SetUp]
        public void SetUp()
        {
            _mainThread = Environment.CurrentManagedThreadId;
        }

        [UnityTest]
        public IEnumerator Dispatcher_Can_Post_On_Main()
        {
            string message = null;
            Task.Run(() => { Dispatcher.Post(() => message = "passed"); });
            yield return new WaitForSeconds(1);
            Assert.That(message, Is.EqualTo("passed"));
        }

        [UnityTest]
        public IEnumerator Dispatcher_Can_Post_Await_On_Main()
        {
            string message = null;
            Task.Run(async () => { await Dispatcher.PostAsync(() => message = "passed"); });
            yield return new WaitForSeconds(1);
            Assert.That(message, Is.EqualTo("passed"));
        }

        [UnityTest]
        public IEnumerator Dispatcher_Can_PostAsync_On_Main()
        {
            string message = null;
            Task.Run(async () => { await Dispatcher.PostAsync(() => message = "passed"); });
            yield return new WaitForSeconds(1);
            Assert.That(message, Is.EqualTo("passed"));
        }

        [UnityTest]
        public IEnumerator Dispatcher_Can_PostAsync_WithResult_On_Main()
        {
            IDispatchResult<string> result = null;
            Task.Run(async () => { result = await Dispatcher.PostAsync(() => "passed"); });
            yield return new WaitForSeconds(1);
            Assert.That(result.Result, Is.EqualTo("passed"));
            Assert.That(result.Succeeded, Is.True);
        }

        [UnityTest]
        public IEnumerator Dispatcher_Can_PostAsync_WithError_On_Main()
        {
            IDispatchResult<string> result = null;
            Task.Run(async () =>
            {
                result = await Dispatcher.PostAsync<string>(() => throw new Exception("test exception"));
            });
            yield return new WaitForSeconds(1);
            Assert.That(result.Result, Is.Null);
            Assert.That(result.Exception, Is.Not.Null);
            Assert.That(result.Succeeded, Is.False);
        }

        [UnityTest]
        public IEnumerator ToMainThread_Runs_On_Thread_Equal_To_MainThreadId()
        {
            Task.Run(async () =>
            {
                Debug.Log($"MainThread = {System.Environment.CurrentManagedThreadId == _mainThread}! ");
                Assert.That(System.Environment.CurrentManagedThreadId, Is.Not.EqualTo(_mainThread));
                await Dispatcher.ToMainThread();
                Assert.That(System.Environment.CurrentManagedThreadId, Is.EqualTo(_mainThread));
                Debug.Log($"MainThread = {System.Environment.CurrentManagedThreadId == _mainThread}! ");
            });
            yield return null;
        }

        [UnityTest]
        public IEnumerator ToMainThread_Does_NotThrow_UnityException()
        {
            Task.Run(async () =>
            {
                await Dispatcher.ToMainThread();
                Assert.DoesNotThrow(() =>
                {
                    var gameObject = new GameObject().AddComponent<Light>();
                    Object.DestroyImmediate(gameObject.gameObject);
                });
            });
            yield return null;
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}