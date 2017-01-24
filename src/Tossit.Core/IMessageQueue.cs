using System;

namespace Tossit.Core
{
    /// <summary>
    /// Interface for message queue implementations.
    /// </summary>
    public interface IMessageQueue
    {
        /// <summary>
        /// Send to queue.
        /// </summary>
        /// <param name="name">Name of queue or whatever using to describing work/job/event. Same as Receive method's name.</param>
        /// <param name="message">Message string to sends to queue.</param>
        /// <param name="options">Options</param>
        /// <returns>Returns true if data send completed successfully, otherwise returns false.</returns>
        /// <exception cref="Exception">Throws when name or data is null.</exception>>
        bool Send(string name, string message, Options options = null);
        /// <summary>
        /// Receive messages from message queue.
        /// This method will register the given function to consume queue.
        /// </summary>
        /// <param name="name">Name of queue or whatever using to describing work/job/event. Same as Send method's name.</param>
        /// <param name="func">Receiver function.</param>
        /// <returns>Returns true if receiver method registered successfully, otherwise returns false.</returns>
        /// <exception cref="Exception">Throws when name or func is null.</exception>>
        bool Receive(string name, Func<string, bool> func);
    }
}