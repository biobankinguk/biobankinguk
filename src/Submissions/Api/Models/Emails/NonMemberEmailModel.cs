namespace Biobanks.Submissions.Api.Models.Emails
{
    public record NonMemberEmailModel(string RecipientName, 
        string BiobankName, 
        string BiobankAnonymousIdentifier, 
        string NetworkName, 
        string NetworkContactEmail, 
        string NetworkDescription);
}
