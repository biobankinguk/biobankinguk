using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
  public class BiobankService : IBiobankService
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public BiobankService(
      UserManager<ApplicationUser> userManager,
      ApplicationDbContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferings(int biobankId)
      => await _db.OrganisationServiceOfferings
        .AsNoTracking()
        .Where(x => x.OrganisationId == biobankId)
        .Include(x => x.ServiceOffering)
        .ToListAsync();

    public async Task<IEnumerable<ApplicationUser>> ListBiobankAdmins(int biobankId)
    {
      var adminIds = await _db.OrganisationUsers
        .AsNoTracking()
        .Where(x => x.OrganisationId == biobankId)
        .Select(x => x.OrganisationUserId)
        .ToListAsync();

      return _userManager.Users.AsNoTracking().Where(x => adminIds.Contains(x.Id));
    }

    public async Task<IEnumerable<Funder>> ListBiobankFunders(int biobankId)
      => await _db.Organisations
        .AsNoTracking()
        .Where(x => x.OrganisationId == biobankId)
        .Include(x => x.Funders)
        .Select(x => x.Funders)
        .FirstOrDefaultAsync();

    public async Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIds(int biobankId)
    {
      // Returns users who have admin role only for this biobank
      var admins = await _db.OrganisationUsers.AsNoTracking().ToListAsync();
      var adminIds = admins.GroupBy(a => a.OrganisationUserId)
        .Where(g => g.Count() == 1)
        .Select(a => a.FirstOrDefault(ai => ai.OrganisationId == biobankId))
        .Select(ou => ou?.OrganisationUserId);

      return await _userManager.Users.Where(x => adminIds.Contains(x.Id)).ToListAsync();
    }

    public async Task<string> GetUnusedTokenByUser(string biobankUserId)
    {
      // Check most recent token record
      var tokenIssue = _db.TokenIssueRecords
        .AsNoTracking()
        .Where(x => x.UserId.Contains(biobankUserId))
        .OrderBy(x => x.IssueDate)
        .FirstOrDefault();

      var tokenValidation = await _db.TokenValidationRecords
        .AsNoTracking()
        .Where(x => x.UserId.Contains(biobankUserId))
        .ToListAsync();

      List<string> token = tokenValidation.Select(t => t.Token).ToList();
      DateTime now = DateTime.Now;

      var user = await _userManager.FindByIdAsync(biobankUserId) ??
                 throw new InvalidOperationException(
                   $"Account could not be confirmed. User not found! User ID: {biobankUserId}");


      if (tokenIssue.Equals(null) || token.Contains(tokenIssue.Token) || tokenIssue.IssueDate < now.AddHours(-20))
      {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
      }
      else
      {
        return tokenIssue.Token;
      }
    }

    public async Task AddBiobankServiceOfferings(IEnumerable<OrganisationServiceOffering> services)
    {
      foreach (var service in services)
      {
        //Validate service id first - don't want to go around inserting new unnamed services
        if (await _db.FindAsync<ServiceOffering>(service.ServiceOfferingId) != null)
        {
          //now make sure the biobank doesn't already have this service listed
          var biobank = await _db.OrganisationServiceOfferings
            .AsNoTracking()
            .Where(x => x.OrganisationId == service.OrganisationId && x.ServiceOfferingId == service.ServiceOfferingId)
            .ToListAsync();

          if (biobank.FirstOrDefault() == null)
          {
            _db.Add(service);
          }
        }
        //atm we just silently fail if the service id is invalid; should we be throwing?
      }

      await _db.SaveChangesAsync();
    }


    public async Task DeleteBiobankServiceOffering(int biobankId, int serviceId)
    {
      //make sure the biobank has this service

      var biobank = await _db.OrganisationServiceOfferings
        .AsNoTracking()
        .Where(x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId)
        .ToListAsync();

      var entity = _db.OrganisationServiceOfferings
        .FirstOrDefault(x => x.OrganisationId == biobankId &&
                             x.ServiceOfferingId == serviceId);

      if (biobank.FirstOrDefault() != null)
      {
        _db.OrganisationServiceOfferings.Remove(entity);
      }

      await _db.SaveChangesAsync();
    }

    public async Task UpdateOrganisationAnnualStatistic(int organisationId, int statisticId, int? value, int year)
    {
      var organisationAnnualStatistic = new OrganisationAnnualStatistic
      {
        OrganisationId = organisationId,
        AnnualStatisticId = statisticId,
        Value = value,
        Year = year
      };

      var organistaionAnnualStats = await _db.OrganisationAnnualStatistics
        .AsNoTracking()
        .Where(x => x.OrganisationId == organisationId && x.AnnualStatisticId == statisticId && x.Year == year)
        .ToListAsync();

      var existingEntry = organistaionAnnualStats.FirstOrDefault();

      if (existingEntry != null)
      {
        existingEntry.Value = value;
        _db.OrganisationAnnualStatistics.Update(existingEntry);
      }

      else
        _db.OrganisationAnnualStatistics.Update(organisationAnnualStatistic);

      await _db.SaveChangesAsync();
    }

    public async Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons)
    {
      foreach (var registrationReason in activeRegistrationReasons)
      {
        //Validate reason id first - don't want to go around inserting new unnamed reasons
        if (await _db.RegistrationReasons.FindAsync(registrationReason.RegistrationReasonId) != null)
        {
          //now make sure the biobank doesn't already have this reason listed
          var biobank = await _db.OrganisationRegistrationReasons
            .Where(x => x.OrganisationId == registrationReason.OrganisationId &&
                        x.RegistrationReasonId == registrationReason.RegistrationReasonId)
            .ToListAsync();

          if (biobank.FirstOrDefault() == null)
          {
            _db.OrganisationRegistrationReasons.Add(registrationReason);
          }
        }
      }

      await _db.SaveChangesAsync();
    }

    public async Task DeleteBiobankRegistrationReason(int organisationId, int registrationReasonId)
    {
      //make sure the biobank has this reason
      var biobank = await _db.OrganisationRegistrationReasons
        .Where(x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId)
        .ToListAsync();

      var entity = _db.OrganisationRegistrationReasons
        .FirstOrDefault(x => x.OrganisationId == organisationId &&
                             x.RegistrationReasonId == registrationReasonId);

      if (biobank.FirstOrDefault() != null)
      {
        _db.OrganisationRegistrationReasons.Remove(entity);
      }

      ;

      await _db.SaveChangesAsync();
    }
  }
}
