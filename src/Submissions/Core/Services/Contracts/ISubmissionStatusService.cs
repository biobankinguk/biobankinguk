using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Core.Exceptions;
using Biobanks.Submissions.Core.Types;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    public interface ISubmissionStatusService
    {
        Task AddErrors(int submissionId, Operation op, string type, ICollection<BiobanksValidationResult> messages, int biobankId);

        Task ProcessRecords(int submissionId, int n);
    }
}
