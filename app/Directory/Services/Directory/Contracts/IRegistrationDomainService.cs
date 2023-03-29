using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Directory.Services.Directory.Contracts
{
    public interface IRegistrationDomainService
    {
        Task<ICollection<RegistrationDomainRule>> ListRules();
        Task<RegistrationDomainRule> GetRuleByValue(string ruleValue);

        Task Add(RegistrationDomainRule registrationDomainRule);
        Task Update(RegistrationDomainRule registrationDomainRule);
        Task Delete(RegistrationDomainRule registrationDomainRule);

        Task<bool> ValidateEmail(string email);
    }
}
