using System;
using System.Threading.Tasks;
using HamerSoft.Threads.Editor;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HamerSoft.Threads.Tests.Editor
{
    [TestFixture]
    public class DispatcherTests
    {
        private EditorUpdateLoop _updater;
        private int _mainThread;

        [SetUp]
        public void SetUp()
        {
            _updater = new EditorUpdateLoop();
            _mainThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        }

        [Test]
        public async Task Dispatcher_Can_Post_On_Main()
        {
            Dispatcher.Start(_updater);
            string message = null;
            await Task.Run(() => { Dispatcher.Post(() => message = "passed"); });
            Assert.That(message, Is.EqualTo("passed"));
        }

        [Test]
        public async Task Dispatcher_Can_Post_Await_On_Main()
        {
            Dispatcher.Start(_updater);
            string message = null;
            await Task.Run(async () => { await Dispatcher.PostAsync(() => message = "passed"); });
            Assert.That(message, Is.EqualTo("passed"));
        }

        [Test]
        public async Task Dispatcher_Can_PostAsync_On_Main()
        {
            Dispatcher.Start(_updater);
            string message = null;
            await Task.Run(async () => { await Dispatcher.PostAsync(() => message = "passed"); });
            Assert.That(message, Is.EqualTo("passed"));
        }

        [Test]
        public async Task Dispatcher_Can_PostAsync_WithResult_On_Main()
        {
            Dispatcher.Start(_updater);
            IDispatchResult<string> result = null;
            await Task.Run(async () => { result = await Dispatcher.PostAsync(() => "passed"); });
            Assert.That(result.Result, Is.EqualTo("passed"));
            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task Dispatcher_Can_PostAsync_WithError_On_Main()
        {
            Dispatcher.Start(_updater);
            IDispatchResult<string> result = null;
            await Task.Run(async () =>
            {
                result = await Dispatcher.PostAsync<string>(() => throw new Exception("test exception"));
            });
            Assert.That(result.Result, Is.Null);
            Assert.That(result.Exception, Is.Not.Null);
            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        public async Task ToMainThread_Runs_On_Thread_Equal_To_MainThreadId()
        {
            Dispatcher.Start(_updater);
            await Task.Run(async () =>
            {
                Assert.That(System.Threading.Thread.CurrentThread.ManagedThreadId, Is.Not.EqualTo(_mainThread));
                await Dispatcher.ToMainThread();
                Assert.That(System.Threading.Thread.CurrentThread.ManagedThreadId, Is.EqualTo(_mainThread));
            });
        }

        [Test]
        public async Task ToBackgroundThread_Runs_On_Thread_NotEqual_To_MainThreadId()
        {
            Dispatcher.Start(_updater);
            await Task.Run(async () =>
            {
                await Dispatcher.ToMainThread();
                Assert.That(System.Threading.Thread.CurrentThread.ManagedThreadId, Is.EqualTo(_mainThread));
                await Dispatcher.ToBackgroundThread();
                Assert.That(System.Threading.Thread.CurrentThread.ManagedThreadId, Is.Not.EqualTo(_mainThread));
            });
        }

        [Test]
        public async Task ToMainThread_Does_NotThrow_UnityException()
        {
            Dispatcher.Start(_updater);
            await Task.Run(async () =>
            {
                await Dispatcher.ToMainThread();
                Assert.DoesNotThrow(() =>
                {
                    var gameObject = new GameObject().AddComponent<Light>();
                    Object.DestroyImmediate(gameObject.gameObject);
                });
            });
        }

        [TearDown]
        public void TearDown()
        {
            Dispatcher.Stop();
            _updater = null;
        }
    }
}