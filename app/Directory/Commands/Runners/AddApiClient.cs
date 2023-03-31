using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.Shared;
using Biobanks.Directory.Commands.Helpers;
using Biobanks.Directory.Utilities.IdentityModel;
using ConsoleTableExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobanks.Directory.Commands.Runners;

internal class AddApiClient
{
  private readonly ILogger<AddApiClient> _logger;
  private readonly IConsole _console;
  private readonly ApplicationDbContext _db;

  public AddApiClient(
    ILoggerFactory logger,
    IConsole console,
    ApplicationDbContext db)
  {
    _logger = logger.CreateLogger<AddApiClient>();
    _console = console;
    _db = db;
  }

  public async Task Run(bool generate, List<int> biobankIds, string clientId, string clientSecret, string clientName)
  {
    #region conditional / dependent argument validation

    if (generate)
    {
      if (!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(clientSecret))
      {
        const string message = "The `--generate` option is exclusive from `--client-id` and `--client-secret`.";
        _logger.LogError(message);
        _console.Error.WriteLine($"Error: {message}");
        return;
      }

      clientId = Crypto.GenerateId();
      clientSecret = Crypto.GenerateId();
    }

    if (string.IsNullOrEmpty(clientId))
    {
      const string message = "One of `--client-id` or `--generate` must be specified.";
      _logger.LogError(message);
      _console.Error.WriteLine($"Error: {message}");
      return;
    }

    if (string.IsNullOrEmpty(clientSecret))
    {
      const string message = "One of `--client-secret` or `--generate` must be specified.";
      _logger.LogError(message);
      _console.Error.WriteLine($"Error: {message}");
      return;
    }

    // Confirm before adding SuperAdmin
    if (!biobankIds.Any())
    {
      var proceed = new Prompt(_console)
        .YesNo("No Biobank Ids were specified. This will create SuperAdmin credentials. Is this correct?");
      if (!proceed) return;
    }

    #endregion

    var apiClient = new ApiClient
    {
      Name = clientName ?? clientId,
      ClientId = clientId,
      ClientSecretHash = clientSecret.Sha256(),
      Organisations = new List<Organisation>()
    };

    foreach (var id in biobankIds)
    {
      var biobank = await _db.Organisations
        .Include(o => o.ApiClients)
        .SingleOrDefaultAsync(x => x.OrganisationId == id);
      if (biobank is null)
      {
        const string message = "Could not find the specified Biobank with ID: {0}";
        _logger.LogError(message, id);
        _console.Error.WriteLine(string.Format($"Error: {message}", id));
        return;
      }

      apiClient.Organisations.Add(biobank);
    }

    await _db.ApiClients.AddAsync(apiClient);
    await _db.SaveChangesAsync();

    // Output
    const string successMessage = "Client `{0}` added successfully to Biobank Ids: {1}";
    _logger.LogInformation(successMessage, clientName ?? clientId, biobankIds);
    _console.Out.WriteLine(string.Format(successMessage, clientName ?? clientId, biobankIds));

    if (generate)
    {
      var outputRows = new List<List<object>>
      {
        new() { "Client ID", clientId },
        new() { "Client Secret", clientSecret }
      };

      _console.Out.Write(ConsoleTableBuilder
        .From(outputRows)
        .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
        .Export()
        .ToString());
    }
  }
}
