using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
{
    public interface IBiobankService
    {
        Task<List<string>> GetOrganisationNames();
    }
}
