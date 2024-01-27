using System;

namespace Hamersoft.Threads
{
    public interface IDispatchResult<out TResult, out TException> where TException : Exception
    {
        TResult Result { get; }
        TException Exception { get; }
        string Message { get; }
    }

    internal class DispatchResult<TResult, TException> : IDispatchResult<TResult, TException>
        where TException : Exception
    {
        public TResult Result { get; internal set; }
        public TException Exception { get; internal set; }
        public string Message { get; internal set; }

        public DispatchResult(TResult result)
        {
            Result = result;
            Exception = default;
            Message = null;
        }

        public DispatchResult(TException exception, string message = null)
        {
            Exception = exception;
            Message = message;
            Result = default;
        }
    }
}