using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Directory.Services.Directory.Contracts;

namespace Biobanks.Directory.Services.Directory
{
    public class TokenLoggingService : ITokenLoggingService
    {
      private readonly ApplicationDbContext _context;

      public TokenLoggingService(

           ApplicationDbContext context
           )
          {
            _context = context;
          }

        /// <summary>
        /// Creates a Token Validation Record to indicate that the token has been through a validation process.
        /// </summary>
        /// <param name="token">The token that was validated</param>
        /// <param name="userId">The user Id the token was validated against</param>
        /// <param name="purpose">The purpose of the token e.g. ConfirmEmail</param>
        /// <returns>An ID for the Token Validation Record, that can be used to update its Validation Success status</returns>
        public async Task<int> TokenValidated(string token, string userId, string purpose)
        {
            var record = new TokenValidationRecord
            {
                Token = token,
                UserId = userId,
                Purpose = purpose,
                ValidationDate = DateTime.Now,
                ValidationSuccessful = false
            };
                _context.Add(record);
            await _context.SaveChangesAsync();

            return record.Id;
        }

        public async Task<int> EmailTokenValidated(string token, string userId)
            => await TokenValidated(token, userId, "ConfirmEmail");

        public async Task<int> PasswordTokenValidated(string token, string userId)
            => await TokenValidated(token, userId, "PasswordReset");

        /// <summary>
        /// Marks the Token Validation Record with the provided Id as Successfully Validated
        /// </summary>
        /// <param name="validationRecordId">The ID of the Token Validation Record to update</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException">Thrown if the provided ID doesn't return a Token Validation Record</exception>
        public async Task ValidationSuccessful(int validationRecordId)
        {
            var record = await _context.FindAsync<TokenValidationRecord>(validationRecordId);

            if (record == null) throw new KeyNotFoundException();

            record.ValidationSuccessful = true;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Records the date and time the provided Token was issued, and for what user and purpose it was generated.
        /// </summary>
        /// <param name="token">The token generated</param>
        /// <param name="userId">The ID of the User the token was generated for</param>
        /// <param name="purpose">The purpose for which the token was generated</param>
        /// <returns></returns>
        public async Task TokenIssued(string token, string userId, string purpose)
        {
            _context.Add(new TokenIssueRecord
            {
                Token = token,
                UserId = userId,
                Purpose = purpose,
                IssueDate = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }

        public async Task EmailTokenIssued(string token, string userId)
            => await TokenIssued(token, userId, "ConfirmEmail");

        public async Task PasswordTokenIssued(string token, string userId)
            => await TokenIssued(token, userId, "ResetPassword");
    }
}

