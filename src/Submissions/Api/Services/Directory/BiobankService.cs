using System;
using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Data;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class BiobankService : IBiobankService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public BiobankService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
          _userManager = userManager;
          _context = context;
        }

        public async Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId)
              => await _context.OrganisationServiceOfferings
                  .AsNoTracking()
                  .Where(x => x.OrganisationId == biobankId)
                  .Include(x => x.ServiceOffering)
                  .ToListAsync(); 

        public async Task<IEnumerable<ApplicationUser>> ListBiobankAdminsAsync(int biobankId)
        {

          var adminIds = await _context.OrganisationUsers
            .AsNoTracking()
            .Where(x => x.OrganisationId == biobankId)
            .Select(x => x.OrganisationUserId)
            .ToListAsync();

          return _userManager.Users.AsNoTracking().Where(x => adminIds.Contains(x.Id));
        }

        public async Task<IEnumerable<Funder>> ListBiobankFundersAsync(int biobankId)
            => await _context.Organisations
                .AsNoTracking()
                .Where(x => x.OrganisationId == biobankId)
                .Include(x => x.Funders)
                .Select(x => x.Funders)
                .FirstOrDefaultAsync();
        
        public async Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIdsAsync(int biobankId)
        {
          // Returns users who have admin role only for this biobank
          // TODO remove the generic repo when upgrading to netcore, as it doesn't support groupby fully
          var admins = await _context.OrganisationUsers.AsNoTracking().ToListAsync();
          var adminIds = admins.GroupBy(a => a.OrganisationUserId)
            .Where(g => g.Count() == 1)
            .Select(a => a.FirstOrDefault(ai => ai.OrganisationId == biobankId))
            .Select(ou => ou?.OrganisationUserId);

          return await _userManager.Users.Where(x => adminIds.Contains(x.Id)).ToListAsync();
        }
        
        public async Task<string> GetUnusedTokenByUser(string biobankUserId)
        {
          // Check most recent token record
          var tokenIssue = _context.TokenIssueRecords
              .AsNoTracking()
              .Where(x => x.UserId.Contains(biobankUserId))
              .OrderBy(x => x.IssueDate)
              .FirstOrDefault();

          var tokenValidation = await _context.TokenValidationRecords
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
    }
}
