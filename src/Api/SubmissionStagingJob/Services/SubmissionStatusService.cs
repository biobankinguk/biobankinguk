using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Common.Data;
using Biobanks.Common.Data.Entities;
using Biobanks.Common.Exceptions;
using Biobanks.Common.Types;
using Biobanks.SubmissionAzureFunction.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.SubmissionAzureFunction.Services
{
    public class SubmissionStatusService : ISubmissionStatusService
    {
        private readonly SubmissionsDbContext _db;
        private readonly IMapper _mapper;

        public SubmissionStatusService(IMapper mapper, SubmissionsDbContext db)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task AddErrors(int submissionId, Operation op, string type, ICollection<BiobanksValidationResult> messages, int biobankId)
        {
            var sub = await _db.Submissions
                .Where(x => x.Id == submissionId)
                .Include(x => x.Errors)
                .SingleOrDefaultAsync();

            foreach (var message in messages)
            {
                sub.Errors.Add(new Error
                {
                    Message = $"Failed to {op} {type}: {message.ErrorMessage}",
                    RecordIdentifiers = message.RecordIdentifiers,
                    SubmissionId = submissionId
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task ProcessRecords(int submissionId, int n)
        {
            var sub = await _db.Submissions.Where(x => x.Id == submissionId).SingleOrDefaultAsync();

            if (sub == null) throw new KeyNotFoundException();

            var newTotalRecordsProcessed = sub.RecordsProcessed + n;
            sub.RecordsProcessed = newTotalRecordsProcessed <= sub.TotalRecords ? newTotalRecordsProcessed : sub.TotalRecords;
            sub.StatusChangeTimestamp = DateTime.Now;

            await _db.SaveChangesAsync();
        }
    }
}
