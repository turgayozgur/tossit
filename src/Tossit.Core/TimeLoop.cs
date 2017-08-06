using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Time loop implementation.
/// </summary>
public class TimeLoop : ITimeLoop, IDisposable
{
    /// <summary>
    /// Wait for miliseconds to start looping.
    /// </summary>
    private readonly int WAIT_TIME_TO_START = 500;
    /// <summary>
    /// Timers that created before.
    /// </summary>
    private readonly IDictionary<Guid, Timer> _timers = new Dictionary<Guid, Timer>();

    /// <summary>
    /// Start new time loop.
    /// </summary>
    /// <param name="action">Action to calling every period. Guid is identifier of time loop.</param>
    /// <param name="period">Loop period.</param>
    /// <returns>Returns identifier of time loop as a Guid. This id can use when you want to stop time loop.</returns>
    public Guid StartNew(Action<Guid> action, int period)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        if (period <= 0) throw new ArgumentException($"{nameof(period)} should be upper than zero.");

        // Create new time loop context.
        var id = Guid.NewGuid();
        var context = new TimeLoopContext
        {
            Id = id,
            Action = action
        };

        // Create a timer that invokes CheckStatus after WAIT_TIME_TO_START, 
        // and every period thereafter.
        var timer = new Timer(Run, context, WAIT_TIME_TO_START, period);

        // Add timer to timer list with an identifier.
        _timers.Add(id, timer);

        return id;
    }

    /// <summary>
    /// Stop timer that identified by given id.
    /// </summary>
    /// <param name="id">Identifier for the time loop instance.</param>
    /// <returns>Returns true if any timer stopped, otherwise returns false.</returns>
    public bool Stop(Guid id)
    {
        if (_timers.ContainsKey(id))
        {
            var timer = _timers[id];
            timer.Dispose();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Run method to call for every timer period.
    /// </summary>
    /// <param name="stateInfo">Context of time loop.</param>
    private void Run(object stateInfo)
    {
        var context = (TimeLoopContext)stateInfo;

        context.Action(context.Id);
    }

    /// <summary>
    /// Stop all timers.
    /// </summary>
    public void Dispose()
    {
        foreach (var timer in _timers)
        {
            timer.Value.Dispose();
        }
    }
}
