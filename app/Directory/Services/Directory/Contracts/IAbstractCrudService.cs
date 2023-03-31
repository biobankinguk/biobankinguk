using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Directory.Areas.Biobank.Models.Collections;
using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Services.Directory.Contracts;

public interface IAbstractCrudService
{
  Task<AbstractCRUDCapabilityModel> PopulateAbstractCRUDAssociatedData(AbstractCRUDCapabilityModel model, bool excludeLinkedData = false);
  
  Task<AbstractCRUDCollectionModel> PopulateAbstractCRUDCollectionModel(AbstractCRUDCollectionModel model, IEnumerable<ConsentRestriction> consentRestrictions = null, bool excludeLinkedData = false);

  Task<AbstractCRUDSampleSetModel> PopulateAbstractCRUDSampleSetModel(AbstractCRUDSampleSetModel model);
}
