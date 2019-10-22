using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO
{
    public class MaterialTypeInboundDto : RefDataBaseDto
    {
        public List<int> MaterialTypeGroupIds { get; set; } = new List<int>();
    }
}
