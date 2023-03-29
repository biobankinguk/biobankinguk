namespace Biobanks.Directory.Models.Shared
{
    public class ReadAssociatedDataTypeGroupModel : AssociatedDataTypeGroupModel
    {
        //Count where AssociatedDataTypeGroup is referenced
        public int AssociatedDataTypeGroupCount { get; set; }
    }
}
