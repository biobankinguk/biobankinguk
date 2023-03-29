using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class RegistrationDomainService : IRegistrationDomainService
    {
        private readonly ApplicationDbContext _db;

        public RegistrationDomainService(ApplicationDbContext db)
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
