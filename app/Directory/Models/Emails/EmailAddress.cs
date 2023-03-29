#nullable enable
namespace Biobanks.Submissions.Api.Models.Emails
{
    public record EmailAddress(string Address)
    {
        public string? Name { get; init; }
    }
}
