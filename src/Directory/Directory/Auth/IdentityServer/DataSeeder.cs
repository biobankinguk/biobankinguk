using System.Linq;
using Common.Data;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;

namespace Directory.Auth.IdentityServer
{
    public static class DataSeeder
    {
        public static void Seed(DirectoryContext context, IConfiguration config)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in TrustedClientData.List(config))
                    context.Clients.Add(client.ToEntity());

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in IdentityResourceData.List())
                    context.IdentityResources.Add(resource.ToEntity());

                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in ApiResourceData.List())
                    context.ApiResources.Add(resource.ToEntity());

                context.SaveChanges();
            }
        }
    }
}
