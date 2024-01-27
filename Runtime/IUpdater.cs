using System;

namespace Hamersoft.Threads
{
    internal interface IUpdater
    {
        public event Action Updated, Stopped;
    }
}