using System;

namespace Tossit.Core
{
    /// <summary>
    /// Time loop interface.
    /// </summary>
    public interface ITimeLoop
    {
        /// <summary>
        /// Start new time loop.
        /// </summary>
        /// <param name="action">Action to calling every period. Guid is identifier of time loop.</param>
        /// <param name="period">Loop period.</param>
        /// <returns>Returns identifier of time loop as a Guid. This id can use when you want to stop time loop.</returns>
        Guid StartNew(Action<Guid> action, int period);
        /// <summary>
        /// Stop timer that identified by given id.
        /// </summary>
        /// <param name="id">Identifier for the time loop instance.</param>
        /// <returns>Returns true if any timer stopped, otherwise returns false.</returns>
        bool Stop(Guid id);
    }
}
