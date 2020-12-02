using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Common.Exceptions;
using Biobanks.Common.Types;

namespace Biobanks.SubmissionProcessJob.Services.Contracts
{
    public interface ISubmissionStatusService
    {
        Task AddErrors(int submissionId, Operation op, string type, ICollection<BiobanksValidationResult> messages, int biobankId);

        Task ProcessRecords(int submissionId, int n);
    }
}
