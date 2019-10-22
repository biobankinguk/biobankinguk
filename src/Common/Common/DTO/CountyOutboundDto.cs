namespace Common.DTO
{
    public class CountyOutboundDto : CountyInboundDto
    {
        public int Id { get; set; }
        public string CountryName { get; set; } = string.Empty;
    }
}
