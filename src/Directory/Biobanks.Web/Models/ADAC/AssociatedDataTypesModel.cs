using System.Collections.Generic;
using Biobanks.Web.Models.Shared;
using System.ComponentModel.DataAnnotations;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Web.Models.ADAC
{
    public class AssociatedDataTypesModel
    {
        public ICollection<AssociatedDataTypeModel> AssociatedDataTypes { get; set; }
        public ICollection<AssociatedDataTypeGroupModel> AssociatedDataTypeGroups { get; set; }
    }

    public class AssociatedDataTypeModel
    {
        [Required]
        public int Id { get; set; }

        public int AssociatedDataTypeGroupId { get; set; }

        public string AssociatedDataTypeGroupName { get; set; }

        [Required]
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public string Message { get; set; }
        public int CollectionCapabilityCount { get; set; }

        public List<OntologyTermModel> OntologyTerms { get; set; }

        public string OntologyTermsJson { get; set; } = "";
    }
}