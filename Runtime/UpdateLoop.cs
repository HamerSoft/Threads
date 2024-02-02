using System;
using UnityEngine;

namespace HamerSoft.Threads
{
    internal class UpdateLoop : MonoBehaviour, IUpdater
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