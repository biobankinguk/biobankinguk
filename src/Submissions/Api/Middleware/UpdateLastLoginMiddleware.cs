using Biobanks.Data;
using Biobanks.Submissions.Api.Constants;
using Biobanks.Submissions.Api.Services.Directory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Middleware
{
    public class DirectoryLoginMiddleware
    {
        private readonly RequestDelegate _next;

        //Updates the users last login datetime upon sucessful auth
        public DirectoryLoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User != null && context.User.Identity.IsAuthenticated)
            {

                using var dbContext = context.RequestServices.GetRequiredService<BiobanksDbContext>();
                var user = dbContext.Users.Where(u => u.UserName == context.User.Identity.Name).FirstOrDefault();
                user.LastLogin = DateTime.Now;
                dbContext.Update(user);
                dbContext.SaveChanges();

                var claims = new List<Claim>
                {
                    new Claim(CustomClaimType.FullName, user.Name),
                    new Claim(CustomClaimType.Email, user.Email)
                };

                // If they're a Biobank Admin then populate the claim for the ID of their Biobank.
                // Additionally, add claims for accepted biobank requests
                if (context.User.IsInRole(Role.BiobankAdmin))
                {
                    //TODO look at using organisationdirectoryservice.getbyuserid
                    //and ListAcceptedRegistrationRequests
                    var organisations = await dbContext.OrganisationUsers.AsNoTracking()
                    .Include(x => x.Organisation)
                    .Where(x => x.OrganisationUserId == user.Id)
                    .Select(x => x.Organisation)
                    .ToListAsync();

                    var organisationsRequests = await dbContext.OrganisationRegisterRequests
                        .AsNoTracking()
                        .Where(x =>
                        x.AcceptedDate != null &&
                        x.DeclinedDate == null &&
                        x.OrganisationCreatedDate == null)
                        .ToListAsync();

                    claims.AddRange(
                    organisations
                        .Select(x => new KeyValuePair<int, string>(x.OrganisationId, x.Name))
                        .Select(x => new Claim(CustomClaimType.Biobank, JsonSerializer.Serialize(x))));

                    claims.AddRange(
                    organisationsRequests
                        .Where(x => x.UserEmail == user.Email)
                        .Select(x => new KeyValuePair<int, string>(x.OrganisationRegisterRequestId, x.OrganisationName))
                        .Select(x => new Claim(CustomClaimType.BiobankRequest, JsonSerializer.Serialize(x))));
                }

                // If they're a Network Admin then populate the claim for the ID of their Network.
                // Additionally, add claims for accepted network requests
                if (context.User.IsInRole(Role.NetworkAdmin))
                {

                }

                var appIdentity = new ClaimsIdentity(claims);
                context.User.AddIdentity(appIdentity);       

                }

//call the next delegate/middleware in the pipeline
await _next(context);
}
}

public static class DirectoryLoginMiddlewareExtension
{
public static IApplicationBuilder UseDirectoryLogin(this IApplicationBuilder builder)
=> builder.UseMiddleware<DirectoryLoginMiddleware>();
}
}
