﻿using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class RegistrationDomainService : IRegistrationDomainService
    {
        private readonly BiobanksDbContext _db;

        public RegistrationDomainService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<ICollection<RegistrationDomainRule>> ListRules()
            => await _db.RegistrationDomainRules.ToListAsync();

        public async Task<RegistrationDomainRule> GetRuleByValue(string ruleValue)
            => await _db.RegistrationDomainRules.FirstOrDefaultAsync(x => x.Value == ruleValue);

        public async Task Add(RegistrationDomainRule registrationDomainRule)
        {

            _db.RegistrationDomainRules.Add(registrationDomainRule);
            await _db.SaveChangesAsync();
        }

        public async Task Update(RegistrationDomainRule registrationDomainRule)
        {
            var rule = await _db.RegistrationDomainRules.FindAsync(registrationDomainRule.Id);
            if (rule == null)
            {
                throw new KeyNotFoundException();
            }
            else
            {
                rule.RuleType = registrationDomainRule.RuleType;
                rule.Source = registrationDomainRule.Source;
                rule.Value = registrationDomainRule.Value;
                rule.DateModified = registrationDomainRule.DateModified;
                await _db.SaveChangesAsync();
            }
        }

        public async Task Delete(RegistrationDomainRule registrationDomainRule)
        {
            var rule = await _db.RegistrationDomainRules.FindAsync(registrationDomainRule.Id);
            if (rule != null)
            {
                _db.RegistrationDomainRules.Remove(rule);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateEmail(string email)
        {
            var rules = await _db.RegistrationDomainRules
                .Where(x => email.EndsWith(x.Value))
                .ToListAsync();

            return rules.Any(x => x.RuleType == "Allow") || !rules.Any(x => x.RuleType == "Block");
        }

    }
}