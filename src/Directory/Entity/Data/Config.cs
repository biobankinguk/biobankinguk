﻿using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class Config
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
        
        public string Name { get; set; }
    
        public string Description { get; set; }

        public bool ReadOnly { get; set; }
    }
}
