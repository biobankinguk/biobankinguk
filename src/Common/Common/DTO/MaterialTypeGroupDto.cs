﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class MaterialTypeGroupDto : RefDataBaseDto
    {
        
        public List<(int MaterialTypeId, string MaterialTypeName)> MaterialTypes { get; set; } = new List<(int MaterialTypeId, string MaterialTypeName)>();
    }
}
