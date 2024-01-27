using System.Threading.Tasks;
using Hamersoft.Threads;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor
{
    [TestFixture]
    public class ToMainThreadTests
    {
        private MockUpdateLoop _updater;
        private int _mainThread;

        [SetUp]
        public void SetUp()
        {
            _updater = new MockUpdateLoop();
            _mainThread = System.Environment.CurrentManagedThreadId;
        }

        [Test]
        public async Task ToMainThread_Does_Not_Throw_UnityException()
        {
            Dispatcher.Start(_updater);
            await Task.Run(async () =>
            {
                Debug.Log($"MainThread = {System.Environment.CurrentManagedThreadId == _mainThread}! ");
                Assert.That(System.Environment.CurrentManagedThreadId, Is.Not.EqualTo(_mainThread));
                await Dispatcher.ToMainThread();
                Assert.That(System.Environment.CurrentManagedThreadId, Is.EqualTo(_mainThread));
                Debug.Log($"MainThread = {System.Environment.CurrentManagedThreadId == _mainThread}! ");
            });
        }

        [TearDown]
        public void TearDown()
        {
            _updater.Stop();
            Dispatcher.Stop();
            _updater = null;
        }
    }
}