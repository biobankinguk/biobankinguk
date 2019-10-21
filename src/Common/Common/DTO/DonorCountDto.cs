using System;
using System.Collections.Generic;
using System.Text;

namespace Common.DTO
{
    public class DonorCountDto : RefDataBaseDto
    {
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }
    }
}
