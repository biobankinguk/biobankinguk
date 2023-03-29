#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Models.Emails;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IEmailSender
    {
        /// <summary>
        /// Send an email (compiled from a Razor View with a Model)
        /// to a single email address,
        /// using the default From account.
        /// </summary>
        /// <typeparam name="TModel">The Type of the ViewModel the View expects</typeparam>
        /// <param name="toAddress">The email address to send to</param>
        /// <param name="viewName">a Razor View to compile to form the email content</param>
        /// <param name="model">a ViewModel instance for the specified View</param>
        /// <param name="ccAddress">a carbon copy for an email address to send to</param>
        Task SendEmail<TModel>(
            EmailAddress toAddress,
            string viewName,
            TModel model,
            EmailAddress? ccAddress = null)
            where TModel : class;

        /// <summary>
        /// Send an email (compiled from a Razor View with a Model)
        /// to multiple addresses,
        /// using the default From account.
        /// </summary>
        /// <typeparam name="TModel">The Type of the ViewModel the View expects</typeparam>
        /// <param name="toAddresses">The email addresses to send to</param>
        /// <param name="viewName">a Razor View to compile to form the email content</param>
        /// <param name="model">a ViewModel instance for the specified View</param>
        /// <param name="ccAddresses">a list of carbon copy for an email address to send to</param>
        Task SendEmail<TModel>(
            List<EmailAddress> toAddresses,
            string viewName,
            TModel model,
            List<EmailAddress>? ccAddresses = null)
            where TModel : class;
    }
}
#nullable disable