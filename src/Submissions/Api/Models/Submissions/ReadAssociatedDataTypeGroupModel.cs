using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Models.Submissions
{
    public class ReadAssociatedDataTypeGroupModel : AssociatedDataTypeGroupModel
    {
        //Count where AssociatedDataTypeGroup is referenced
        public int AssociatedDataTypeGroupCount { get; set; }
    }
}
