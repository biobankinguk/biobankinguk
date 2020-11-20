
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadAssociatedDataTypeGroupModel : AssociatedDataTypeGroupModel
    {
        //Count where AssociatedDataTypeGroup is referenced
        public int AssociatedDataTypeGroupCount { get; set; }
    }
}