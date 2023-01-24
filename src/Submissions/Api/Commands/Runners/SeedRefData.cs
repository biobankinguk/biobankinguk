using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Entities.Shared;
using Biobanks.Submissions.Api.Utilities.IdentityModel;
using ConsoleTableExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Biobanks.Submissions.Api.Commands.Runners
{
  internal class SeedRefData
  {
    private readonly ILogger<SeedRefData> _logger;
    private readonly IConsole _console;
    private readonly ApplicationDbContext _db;

    public SeedRefData(
      ILoggerFactory logger,
      IConsole console,
      ApplicationDbContext db)
    {
      _logger = logger.CreateLogger<SeedRefData>();
      _console = console;
      _db = db;
    }

    public async Task Run(string dataDirectory)
    {
      
      
    }
  }
}
