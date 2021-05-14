using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    public interface IRegistrationDomainService
    {
        Task Add(RegistrationDomainRule registrationDomainRule);
        Task Update(RegistrationDomainRule registrationDomainRule);
        Task Delete(RegistrationDomainRule registrationDomainRule);

        Task<bool> ValidateRegistrationEmailAddress(string email);
        Task<ICollection<RegistrationDomainRule>> ListRegistrationDomainRulesAsync();
        Task<RegistrationDomainRule> GetRegistrationDomainRuleByValueAsync(string ruleValue);
    }
}