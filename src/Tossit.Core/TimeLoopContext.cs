using System;

/// <summary>
/// Time loop context
/// </summary>
public class TimeLoopContext
{
    /// <summary>
    /// Id of the timer to stop looping.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// The action to call every timer period.
    /// </summary>
    public Action<Guid> Action { get; set; }
}