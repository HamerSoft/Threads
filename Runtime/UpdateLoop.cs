using System;
using UnityEngine;

namespace Hamersoft.Threads
{
    public class UpdateLoop : MonoBehaviour, IUpdater
    {
        public event Action Updated, Stopped;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            Updated?.Invoke();
        }

        private void OnDestroy()
        {
            Stopped?.Invoke();
        }
    }
}