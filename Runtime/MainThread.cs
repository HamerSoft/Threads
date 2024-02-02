using System;
using System.Threading;

namespace HamerSoft.Threads
{
    internal sealed class MainThread
    {
        private SynchronizationContext UnitySynchronizationContext { get; }

        internal MainThread()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
        }

        internal void Run(Action action)
        {
            if (SynchronizationContext.Current == UnitySynchronizationContext)
            {
                action();
            }
            else
            {
                UnitySynchronizationContext.Post(_ => action(), null);
            }
        }
    }
}