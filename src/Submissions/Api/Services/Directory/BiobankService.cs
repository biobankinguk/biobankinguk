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

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class BiobankService : IBiobankService
    {
        private readonly IGenericEFRepository<Organisation> _organisationRepository;
        private readonly IGenericEFRepository<OrganisationServiceOffering> _organisationServiceOfferingRepository;
        private readonly IGenericEFRepository<OrganisationUser> _organisationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericEFRepository<TokenValidationRecord> _tokenValidationRecordRepository;
        private readonly IGenericEFRepository<TokenIssueRecord> _tokenIssueRecordRepository;


        public BiobankService(
            IGenericEFRepository<Organisation> organisationRepository,
            IGenericEFRepository<OrganisationServiceOffering> organisationServiceOfferingRepository,
            IGenericEFRepository<OrganisationUser> organisationUserRepository,
            UserManager<ApplicationUser> userManager, IGenericEFRepository<TokenIssueRecord> tokenIssueRecordRepository, IGenericEFRepository<TokenValidationRecord> tokenValidationRecordRepository)
        {
            _organisationRepository = organisationRepository;
            _organisationServiceOfferingRepository = organisationServiceOfferingRepository;
            _organisationUserRepository = organisationUserRepository;
            _userManager = userManager;
            _tokenIssueRecordRepository = tokenIssueRecordRepository;
            _tokenValidationRecordRepository = tokenValidationRecordRepository;
        }

        public async Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId)
            => await _organisationServiceOfferingRepository.ListAsync(
                false,
                x => x.OrganisationId == biobankId,
                null,
                x => x.ServiceOffering);

        public async Task<IEnumerable<ApplicationUser>> ListBiobankAdminsAsync(int biobankId)
        {
          var adminIds = (await _organisationUserRepository.ListAsync(
              false,
              x => x.OrganisationId == biobankId))
              .Select(x => x.OrganisationUserId);

          return _userManager.Users.AsNoTracking().Where(x => adminIds.Contains(x.Id));
        }
        
        public async Task<IEnumerable<Funder>> ListBiobankFundersAsync(int biobankId)
            => (await _organisationRepository.ListAsync(
                    false,
                    x => x.OrganisationId == biobankId,
                    null,
                    x => x.Funders))
                .Select(x => x.Funders)
                .FirstOrDefault();
        
        public async Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIdsAsync(int biobankId)
        {
          // Returns users who have admin role only for this biobank
          // TODO remove the generic repo when upgrading to netcore, as it doesn't support groupby fully
          var admins = await _organisationUserRepository.ListAsync(false);
          var adminIds = admins.GroupBy(a => a.OrganisationUserId)
            .Where(g => g.Count() == 1)
            .Select(a => a.FirstOrDefault(ai => ai.OrganisationId == biobankId))
            .Select(ou => ou?.OrganisationUserId);

          return await _userManager.Users.Where(x => adminIds.Contains(x.Id)).ToListAsync();
        }
        
        public async Task<string> GetUnusedTokenByUser(string biobankUserId)
        {
          // Check most recent token record
          var tokenIssue = (await _tokenIssueRecordRepository.ListAsync(
            false,
            x => x.UserId.Contains(biobankUserId),
            x => x.OrderBy(c => c.IssueDate))).FirstOrDefault();

          // Check validation records
          var tokenValidation = await _tokenValidationRecordRepository.ListAsync(
            false,
            x => x.UserId.Contains(biobankUserId));

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
