namespace Tossit.WorkQueue.Job
{
    /// <summary>
    /// Job name validator interface for job name validator implementations.
    /// </summary>
    public interface IJobNameValidator
    {
        /// <summary>
        /// Validate given job name.
        /// </summary>
        /// <param name="jobName">Job name to needs to validate.</param>
        /// <returns>If job name is valid returns true, otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">Throws when jobName is null or empty.</exception>>
        /// <exception cref="System.Exception">Throws when jobName not valid for dot notation and lower case.</exception>>
        bool Validate(string jobName);
    }
}