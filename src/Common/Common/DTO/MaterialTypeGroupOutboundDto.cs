using System.Collections.Generic;

namespace Common.DTO
{
    public class MaterialTypeGroupOutboundDto : RefDataBaseDto
    {     
        public int Id { get; set; }
        public List<MaterialTypeChildDto> MaterialTypes { get; set; } = new List<MaterialTypeChildDto>();
    }

    /// <summary>
    /// Child DTO for MaterialTypeGroup to help with mapping values from MaterialTypeGroups to MaterialTypeDto
    /// </summary>
    public class MaterialTypeChildDto
    {
        public int MaterialTypeId { get; set; }
        public string MaterialTypeName { get; set; } = string.Empty;
    }
}
