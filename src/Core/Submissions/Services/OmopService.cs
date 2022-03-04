using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Omop.Context;
using Biobanks.Omop.Entities;
using Core.Submissions.Services.Contracts;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class OmopService : IOmopService
{
        // Automapper
        private readonly IMapper _mapper;
        private readonly OmopDbContext _db;


        /// <inheritdoc />
        public OmopService(OmopDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ConditionOccurence>> ListSynonyms(int snomedCode)
            => await _db.ConditionOccurences.Where(c => c.ConditionConceptId == snomedCode).ToListAsync();
        

}
