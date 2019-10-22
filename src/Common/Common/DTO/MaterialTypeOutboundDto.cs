﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class MaterialTypeOutboundDto : RefDataBaseDto
    {
        public int Id { get; set; }
        [Required]
        public List<MaterialTypeGroupChildDto> MaterialTypeGroups { get; set; } = new List<MaterialTypeGroupChildDto>();
    }

    /// <summary>
    /// Child DTO for MaterialType to help with mapping values from MaterialTypeGroups to MaterialTypeDto
    /// </summary>
    public class MaterialTypeGroupChildDto
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }
}
