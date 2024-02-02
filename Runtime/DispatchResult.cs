using System;

namespace HamerSoft.Threads
{
    internal class DispatchResult<TResult> : IDispatchResult<TResult>
    {
        /// <inheritdoc/>
        public TResult Result { get; internal set; }
        /// <inheritdoc/>
        public bool Succeeded => Exception == null;
        /// <inheritdoc/>
        public Exception Exception { get; internal set; }

        internal DispatchResult(TResult result)
        {
            Result = result;
            Exception = null;
        }

        internal DispatchResult(Exception exception)
        {
            Exception = exception;
            Result = default;
        }
    }
}