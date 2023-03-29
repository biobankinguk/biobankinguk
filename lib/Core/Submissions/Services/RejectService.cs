using System;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Submissions.Services.Contracts;
using Biobanks.Submissions.Types;
using Z.EntityFramework.Plus;

namespace Biobanks.Submissions.Services
{
    /// <inheritdoc />
    public class RejectService : IRejectService
    {
        private readonly ApplicationDbContext _db;

        /// <inheritdoc />
        public RejectService(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task RejectStagedData(int organisationId)
        {
            // remove all of the submitted data
            await _db.StagedDiagnoses.Where(sd => sd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedSamples.Where(ss => ss.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedTreatments.Where(st => st.OrganisationId == organisationId).DeleteAsync();

            // remove all of the submitted deletes
            await _db.StagedDiagnosisDeletes.Where(sdd => sdd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedSampleDeletes.Where(ssd => ssd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedTreatmentDeletes.Where(std => std.OrganisationId == organisationId).DeleteAsync();

            // mark the submissions as rejected
            var rejected = _db.Statuses.FirstOrDefault(s => s.Value == Statuses.Rejected);

            foreach (var submission in _db.Submissions.Where(s => s.BiobankId == organisationId))
            {
                submission.StatusChangeTimestamp = DateTime.Now;
                submission.Status = rejected;
            }

            // save the submission changes to db
            await _db.SaveChangesAsync();
        }
    }

}
