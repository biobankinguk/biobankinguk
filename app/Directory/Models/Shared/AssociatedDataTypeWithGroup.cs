using System.Collections.Generic;

namespace Biobanks.Directory.Models.Shared
{
    //Added this model to keep front end functionality, should revist as part of the detailed refactor
    public class AssociatedDataTypeWithGroup
    {
        public List<AssociatedDataTypeModel> AssociatedDataTypes { get; set;}
        public List<AssociatedDataTypeGroupModel> AssociatedDataTypeGroups { get; set;}

    }
}
