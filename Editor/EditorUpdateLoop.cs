using System;
using UnityEditor;

namespace HamerSoft.Threads.Editor
{
    internal class EditorUpdateLoop : IUpdater
    {
        public event Action Updated;
        public event Action Stopped;

        public EditorUpdateLoop()
        {
            EditorApplication.update += OnUpdated;
            EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
        }

        private void EditorApplicationOnplayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Dispatcher.Start(this);
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    Dispatcher.Stop();
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnUpdated()
        {
            Updated?.Invoke();
        }
    }
}