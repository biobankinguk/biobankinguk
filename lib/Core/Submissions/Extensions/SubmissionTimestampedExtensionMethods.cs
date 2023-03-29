using System.Collections.Generic;
using Biobanks.Entities.Api.Contracts;
using Core.Submissions.Exceptions;

namespace Core.Submissions.Extensions
{
    public static class SubmissionTimestampedExtensionMethods
    {
        /// <summary>
        /// Compares a collection of ISubmissionTimestamped objects to the source object's timestamp and throws an exception if the source object is not the newest item
        /// </summary>
        /// <param name="timestampedSource">this</param>
        /// <param name="timestampedCollection">A Collection of ISubmissionTimestamped objects to compare against</param>
        public static void EnsureNewerTimestampThan(this ISubmissionTimestamped timestampedSource,
            IEnumerable<ISubmissionTimestamped> timestampedCollection)
        {
            foreach (var timestamped in timestampedCollection)
            {
                timestampedSource.EnsureNewerTimestampThan(timestamped);
            }
        }

        /// <summary>
        /// Compares an ISubmissionTimestamped object to the source object's timestamp and throws an exception if the source object is not the newest item
        /// </summary>
        /// <param name="timestampedSource">this</param>
        /// <param name="compareTarget">An ISubmissionTimestamped object to compare against</param>
        public static void EnsureNewerTimestampThan(this ISubmissionTimestamped timestampedSource,
            ISubmissionTimestamped compareTarget)
        {
            if (compareTarget.SubmissionTimestamp > timestampedSource.SubmissionTimestamp)
                throw new NewerRecordExistsException();
        }
    }
}
