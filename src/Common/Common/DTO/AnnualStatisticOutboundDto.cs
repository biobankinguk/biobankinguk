namespace Common.DTO
{
    public class AnnualStatisticOutboundDto : AnnualStatisticInboundDto
    {
        public int Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }
}
