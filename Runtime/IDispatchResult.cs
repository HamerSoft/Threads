using System;

namespace HamerSoft.Threads
{
    /// <summary>
    /// Result object of a function executed on the main-thread
    /// </summary>
    /// <typeparam name="TResult">result of the Func<T></typeparam>
    public interface IDispatchResult<out TResult>
    {
        /// <summary>
        /// The return value of the function
        /// </summary>
        public TResult Result { get; }

        /// <summary>
        /// Completion flag of the function
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Optional exception throw, when unsuccessful
        /// </summary>
        public Exception Exception { get; }
    }
}