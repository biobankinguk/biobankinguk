﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services
{
    public interface IPublicationService
    {
        Task AddOrganisationPublications(int organisationId, IEnumerable<PublicationDto> publications);
    }
}
