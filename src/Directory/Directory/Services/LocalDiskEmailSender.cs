using System;
using System.Threading.Tasks;
using Directory.Config;
using Microsoft.Extensions.Options;

namespace Directory.Services
{
    public class LocalDiskEmailSender : IEmailSender
    {
        private readonly LocalMailOptions _config;

        public string FromName { get; set; }
        public string FromAddress { get; set; }

        public LocalDiskEmailSender(IOptions<LocalMailOptions> options)
        {
            _config = options.Value;
            FromName = _config.FromName;
            FromAddress = _config.FromAddress;
        }

        public Task SendEmail<TModel>(string toAddress, string subject, string viewName, TModel model)
        {
            throw new NotImplementedException();
        }
    }
}
