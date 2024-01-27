using UnityEditor;

namespace HamerSoft.Threads.Editor
{
    static class EditorDispatcher
    {
        private static EditorUpdateLoop _updater;
        
        [InitializeOnLoadMethod]
        private static void Start()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (_updater == null)
            {
                _updater = new EditorUpdateLoop();
            }
        }
    }
}