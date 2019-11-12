﻿using System.Threading.Tasks;

namespace Upload.Contracts
{
    /// <summary>
    /// Service for handling committing submissions.
    /// </summary>
    public interface ICommitService
    {
        /// <summary>
        /// Commits the 'open' submissions for the organisation.
        /// </summary>
        /// <param name="replace">True to replace all existing data for the organisation, or false to merge the two.</param>
        /// /// <param name="organisationId">Unique identifier of the organisation (organisation) on which to operate.</param>
        Task CommitStagedData(bool replace, int organisationId);
    }
}