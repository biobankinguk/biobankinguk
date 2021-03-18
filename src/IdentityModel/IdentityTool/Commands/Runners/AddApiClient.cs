using Biobanks.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.CommandLine;
using System.Threading.Tasks;

namespace Biobanks.IdentityTool.Commands.Runners
{
    internal class AddApiClient
    {
        private readonly ILogger<AddApiClient> _logger;
        private readonly BiobanksDbContext _db;

        public AddApiClient(
            ILogger<AddApiClient> logger,
            BiobanksDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task Run(IConsole console, bool generate)
        {
            var test = await _db.OrganisationTypes.AsNoTracking().FirstAsync();
            _logger.LogInformation($"hello: {test.Description}");

            //_logger.LogInformation(
            //    $"Hashing {{{nameof(input)}}} with SHA256 as {{hashFormat}}",
            //    input,
            //    OutputFormat.Base64Url);

            //var outputRows = new List<List<object>>
            //{
            //    new() { "Input", input },
            //    new() { "SHA256", input.Sha256() }
            //};

            //console.Out.Write(ConsoleTableBuilder
            //    .From(outputRows)
            //    .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
            //    .Export()
            //    .ToString());
        }
    }
}

