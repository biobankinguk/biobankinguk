namespace Biobanks.Directory.Models.Emails

{
    /// <summary>
    /// Model that covers most emails sending token links
    /// e.g. Password Reset, Account Confirmation etc...
    /// </summary>
    /// <param name="RecipientName">Name of the recipient</param>
    /// <param name="ActionLink">Link for the Primary Action</param>
    /// <param name="ResendLink">Link to resend email with a fresh token link</param>
    ///
    public record TokenEmailModel(
        string RecipientName,
        string ActionLink,
        string ResendLink);
}
