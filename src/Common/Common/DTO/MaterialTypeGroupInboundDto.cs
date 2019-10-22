using System.Collections.Generic;

namespace Common.DTO
{
    public class MaterialTypeGroupInboundDto : RefDataBaseDto
    {
        public List<int> MaterialTypeIds { get; set; } = new List<int>();
    }
}
