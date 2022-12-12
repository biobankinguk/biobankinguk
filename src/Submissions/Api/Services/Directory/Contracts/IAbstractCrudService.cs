using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface IAbstractCrudService
{
  Task<ActionResult> PopulateAbstractCRUDAssociatedData(AbstractCRUDCapabilityModel model, bool excludeLinkedData = false);
}
