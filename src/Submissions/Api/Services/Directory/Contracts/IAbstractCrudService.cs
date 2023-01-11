using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Areas.Biobank.Models.Collections;
using Biobanks.Submissions.Api.Models.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts;

public interface IAbstractCrudService
{
  Task<AbstractCRUDCapabilityModel> PopulateAbstractCRUDAssociatedData(AbstractCRUDCapabilityModel model, bool excludeLinkedData = false);
  
  Task<AbstractCRUDCollectionModel> PopulateAbstractCRUDCollectionModel(AbstractCRUDCollectionModel model, IEnumerable<ConsentRestriction> consentRestrictions = null, bool excludeLinkedData = false);

  Task<AbstractCRUDSampleSetModel> PopulateAbstractCRUDSampleSetModel(AbstractCRUDSampleSetModel model);
}
