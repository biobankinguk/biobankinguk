using Biobanks.Data;
using Biobanks.Entities.Shared;
using Biobanks.IdentityModel.Helpers;
using Biobanks.IdentityModel.Extensions;

using Microsoft.Extensions.Logging;

using System.CommandLine;
using System.CommandLine.IO;
using System.Threading.Tasks;
using ConsoleTableExt;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Biobanks.IdentityTool.Commands.Runners
{
    internal class AddApiClient
    {
        private readonly ILogger<AddApiClient> _logger;
        private readonly ApplicationDbContext _db;

        public AddApiClient(
            ILogger<AddApiClient> logger,
            ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task Run(IConsole console, bool generate, List<int> biobankIds, string clientId, string clientSecret, string clientName)
        {
            #region conditional / dependent argument validation


            if (generate)
            {
                if (!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(clientSecret))
                {
                    var message = "The `--generate` option is exclusive from `--client-id` and `--client-secret`.";
                    _logger.LogError(message);
                    console.Error.WriteLine($"Error: {message}");
                    return;
                }

                clientId = Crypto.GenerateId();
                clientSecret = Crypto.GenerateId();
            }

            if (string.IsNullOrEmpty(clientId))
            {
                var message = "One of `--client-id` or `--generate` must be specified.";
                _logger.LogError(message);
                console.Error.WriteLine($"Error: {message}");
                return;
            }

            if (string.IsNullOrEmpty(clientSecret))
            {
                var message = "One of `--client-secret` or `--generate` must be specified.";
                _logger.LogError(message);
                console.Error.WriteLine($"Error: {message}");
                return;
            }

            // Confirm before adding SuperAdmin
            if (!biobankIds.Any())
            {
                bool? proceed = null;
                while (proceed is null)
                {
                    console.Out.WriteLine("No Biobank Ids were specified. This will create SuperAdmin credentials. Is this correct? (y/N)");
                    var keyInfo = System.Console.ReadKey();
                    proceed = keyInfo.Key switch
                    {
                        System.ConsoleKey.Y => true,
                        System.ConsoleKey.N => false,
                        System.ConsoleKey.Enter => false,
                        _ => null
                    };
                }
                if (!proceed.GetValueOrDefault()) return;
            }

            #endregion

            var apiClient = new ApiClient
            {
                Name = clientName ?? clientId,
                ClientId = clientId,
                ClientSecretHash = clientSecret.Sha256(),
                Organisations = new List<Entities.Data.Organisation>()
            };

            foreach (var id in biobankIds)
            {
                var biobank = await _db.Organisations
                    .Include(o => o.ApiClients)
                    .SingleOrDefaultAsync(x => x.OrganisationId == id);
                if (biobank is null)
                {
                    var message = "Could not find the specified Biobank with ID: {0}";
                    _logger.LogError(message, id);
                    console.Error.WriteLine(string.Format($"Error: {message}", id));
                    return;
                }

                apiClient.Organisations.Add(biobank);
            }

            await _db.ApiClients.AddAsync(apiClient);
            await _db.SaveChangesAsync();

            // Output
            var successMessage = "Client `{0}` added successfully to Biobank Ids: {1}";
            _logger.LogInformation(successMessage, clientName ?? clientId, biobankIds);
            console.Out.WriteLine(string.Format(successMessage, clientName ?? clientId, biobankIds));

            if (generate)
            {
                var outputRows = new List<List<object>>
                {
                    new() { "Client ID", clientId },
                    new() { "Client Secret", clientSecret }
                };

                console.Out.Write(ConsoleTableBuilder
                    .From(outputRows)
                    .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
                    .Export()
                    .ToString());
            }
        }
    }
}

