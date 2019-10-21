using System;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class AnnualStatisticOutboundDto : AnnualStatisticInboundDto
    {
        public string GroupName { get; set; } = string.Empty;
    }
}
