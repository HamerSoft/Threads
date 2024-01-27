using System;

namespace HamerSoft.Threads
{
    internal interface IUpdater
    {
        public event Action Updated, Stopped;
    }
}