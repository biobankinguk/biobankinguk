using System.Linq;
using AutoMapper.Configuration;
using Common.Data;
using IdentityServer4.EntityFramework.Mappers;

namespace Directory.IdentityServer
{
    public static class ConfigDataSeeder
    {
        public static void SeedIdentityServerConfig(DirectoryContext context, IConfiguration config)
        {
            if (!context.Clients.Any())
            {
                foreach (var client in GetClients(config))
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
