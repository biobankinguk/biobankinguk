﻿using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Services.Contracts
{
    [Obsolete("To be deleted when the Directory core version goes live." +
      " Any changes made here will need to be made in the corresponding core version"
      , false)]
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