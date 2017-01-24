using System;
using System.Text.RegularExpressions;

namespace Tossit.WorkQueue.Job
{
    /// <summary>
    /// Job name validator.
    /// </summary>
    public class JobNameValidator : IJobNameValidator
    {
        /// <summary>
        /// Validate given job name.
        /// </summary>
        /// <param name="jobName">Job name to needs to validate.</param>
        /// <returns>If job name is valid returns true, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Throws when jobName is null or empty.</exception>>
        /// <exception cref="Exception">Throws when jobName not valid for dot notation and lower case.</exception>>
        public bool Validate(string jobName)
        {
            if (string.IsNullOrWhiteSpace(jobName))
            {
                throw new ArgumentNullException(nameof(jobName));
            }

            if (!Regex.IsMatch(jobName, @"^([a-z]+((\.\w)|$))+$"))
            {
                throw new Exception("Job name should be valid for lower case dot notation. e.g: tossit.job.name");
            }

            return true;
        }
    }
}