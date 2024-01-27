using System;
using Hamersoft.Threads;
using UnityEditor;

namespace Tests.Editor
{
    internal class MockUpdateLoop : IUpdater
    {
        public event Action Updated;
        public event Action Stopped;

        public MockUpdateLoop()
        {
            EditorApplication.update += OnUpdated;
        }

        public void Stop()
        {
            EditorApplication.update -= OnUpdated;
            Stopped?.Invoke();
        }

        private void OnUpdated()
        {
            Updated?.Invoke();
        }
    }
}