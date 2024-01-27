using UnityEditor;

namespace Hamersoft.Threads
{
    [InitializeOnLoad]
    public static class EditorDispatcher
    {
        private static readonly EditorUpdateLoop _updater;

        static EditorDispatcher()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (_updater == null)
            {
                _updater = new EditorUpdateLoop();
            }
        }
    }
}