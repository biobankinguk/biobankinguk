namespace Biobanks.Directory.Models.Emails
{
    public record NonMemberEmailModel(string RecipientName, 
        string BiobankName, 
        string BiobankAnonymousIdentifier, 
        string NetworkName, 
        string NetworkContactEmail, 
        string NetworkDescription);
}
