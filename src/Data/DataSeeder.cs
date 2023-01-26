using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Data;

public class DataSeeder
{
  private readonly ApplicationDbContext _dbContext;

  public DataSeeder(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task SeedConfigs()
  {


    if (!await _dbContext.Configs
      .AsNoTracking()
      .AnyAsync())
    {
      var seedConfig = new List<Config>
      {  };
    }
  }

}
