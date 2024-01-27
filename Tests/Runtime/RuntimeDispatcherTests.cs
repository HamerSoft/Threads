using System;
using System.Collections;
using System.Threading;
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
        private Thread _thead;

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
            _thead = new Thread(async () =>
            {
                Assert.That(Thread.CurrentThread.ManagedThreadId, Is.Not.EqualTo(_mainThread));
                await Dispatcher.ToMainThread();
                Assert.That(Thread.CurrentThread.ManagedThreadId, Is.EqualTo(_mainThread));
                _thead?.Abort();
                _thead = null;
            });
            _thead.Start();
            yield return new WaitForSeconds(1);
        }
        
        [UnityTest]
        public IEnumerator ToBackgroundThread_Runs_On_Thread_NotEqual_To_MainThreadId()
        {
            _thead = new Thread(async () =>
            {
                await Dispatcher.ToMainThread();
                Assert.That(Thread.CurrentThread.ManagedThreadId, Is.EqualTo(_mainThread));
                await Dispatcher.ToBackgroundThread();
                Assert.That(Thread.CurrentThread.ManagedThreadId, Is.Not.EqualTo(_mainThread));
                _thead?.Abort();
                _thead = null;
            });
            _thead.Start();
            yield return new WaitForSeconds(1);
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