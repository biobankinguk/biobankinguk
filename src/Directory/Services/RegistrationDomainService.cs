using System;
using System.Threading.Tasks;
using Biobanks.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Directory.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Collections.Generic;
using System.Text;
using Biobanks.Directory.Data.Repositories;


namespace Biobanks.Services
{
    public class RegistrationDomainService : IRegistrationDomainService
    {
        private readonly BiobanksDbContext _db;

        public RegistrationDomainService(BiobanksDbContext db)
        {
            _db = db;
        }

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

        public async Task<bool> ValidateRegistrationEmailAddress(string email)
        {
            //Check the allow list, if no match is found determine if in the block list.
            if ((await _db.RegistrationDomainRules.ToListAsync()).Where(x => x.RuleType == "Allow" && email.Contains(x.Value)).Any())
            {
                return true;
            }
            else
            {
                //If the condition is true (A Block rule which applies is found), then we need to invert the response.
                return !(await _db.RegistrationDomainRules.ToListAsync()).Where(x => x.RuleType == "Block" && email.Contains(x.Value)).Any();
            }
        }

        public async Task<ICollection<RegistrationDomainRule>> ListRegistrationDomainRulesAsync() =>
            await _db.RegistrationDomainRules.ToListAsync();

        public async Task<RegistrationDomainRule> GetRegistrationDomainRuleByValueAsync(string ruleValue) =>
            await _db.RegistrationDomainRules.FirstOrDefaultAsync(x => x.Value == ruleValue);

    }
}
