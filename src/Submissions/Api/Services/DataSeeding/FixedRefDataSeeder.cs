using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Constants;
using Microsoft.AspNetCore.Identity;

namespace Biobanks.Submissions.Api.Services.DataSeeding;

public class FixedRefDataSeeder
{
  private readonly ApplicationDbContext _db;
  private readonly RoleManager<IdentityRole> _roles;

  public FixedRefDataSeeder(ApplicationDbContext db, RoleManager<IdentityRole> roles)
  {
    _db = db;
    _roles = roles;
  }

  public async Task Seed()
  {
    await SeedRoles();
    await SeedDirectoryConfigs();
    await SeedOrganisationTypes();
    await SeedContentPages();
  }

  public async Task SeedDirectoryConfigs()
  {
    var set = _db.Set<Entities.Data.Config>();

    var defaults = new List<Entities.Data.Config>
    {
      #region Display

      // Show Analytics View
      new()
      {
        Key = "site.display.analytics",
        Value = "true",
        Name = "Show Analytics",
        Description = "Enable/Disable Analytics View",
        ReadOnly = false
      },
      // Show Biobank Network Approval
      new()
      {
        Key = "site.display.trusted.biobanks",
        Value = "true",
        Name = "Require Biobank Approval",
        Description = "Enable/Disable Biobank approval to join a Network",
        ReadOnly = false
      },
      // Show Counties
      new()
      {
        Key = "site.display.counties",
        Value = "true",
        Name = "Use counties",
        Description = "Enable/Disable counties",
        ReadOnly = true
      },
      // Show Funders Free Text
      new()
      {
        Key = "site.display.funders.freetext",
        Value = "false",
        Name = "Funders Free Text Input",
        Description = "Enable/Disable free text input for Funders",
        ReadOnly = false
      },
      // Show Live Chat
      new()
      {
        Key = "site.display.livechat",
        Value = "true",
        Name = "Show live chat",
        Description = "Enable/Disable live chat on all pages",
        ReadOnly = false
      },
      // Show Publications View
      new()
      {
        Key = "site.display.publications",
        Value = "true",
        Name = "Show Publications",
        Description = "Enable/Disable Publications View",
        ReadOnly = false
      },
      // Show Submissions View
      new()
      {
        Key = ConfigKey.DisplaySubmissions,
        Value = "true",
        Name = "Show Submissions API Config",
        Description = "Enable/Disable Submissions API Config View"
      },
      // Show Preservation Type Percentage
      new()
      {
        Key = "site.display.preservation.percent",
        Value = "true",
        Name = "Show preservation detail percentage",
        Description = "Enable/Disable preservation detail percentage",
        ReadOnly = false
      },
      // Show Third Party Contact Link
      new()
      {
        Key = "site.display.thirdparty",
        Value = "true",
        Name = "Allow third party contact",
        Description = "Enable/Disable third party contact in Contact List",
        ReadOnly = false
      },
      new()
      {
        Key = "site.display.collectionsnotes",
        Value = "false",
        Name = "Show notes field for Collections",
        Description = "Display notes text field when adding a Collection",
        ReadOnly = false
      },
      // Storage Temperature Name Override
      new()
      {
        Key = "site.display.storagetemperature.name",
        Value = "Storage Temperature",
        Name = "Set Storage Temperatures Custom Name",
        Description = "Set Storage Temperatures Custom Name",
        ReadOnly = true
      },
      // Donor Count Name Override
      new()
      {
        Key = "site.display.donorcount.name",
        Value = "Donor Count",
        Name = "Set Donor Count Custom Name",
        Description = "Set Donor Count Custom Name",
        ReadOnly = true
      },
      // Macroscopic Assessment Name Override
      new()
      {
        Key = "site.display.macroscopicassessment.name",
        Value = "Macroscopic Assessment",
        Name = "Set Macroscopic Assessments Custom Name",
        Description = "Set Macroscopic Assessments Custom Name",
        ReadOnly = true
      },

      #endregion

      #region Home Page

      new()
      {
        Key = "site.homepage.title",
        Value = "Biobanking Directory",
      },
      new()
      {
        Key = "site.homepage.searchtitle",
        Value = "Search the Directory",
      },
      new()
      {
        Key = "site.homepage.searchsubtitle",
        Value = "Search for existing banked samples or for a group to collect samples on your behalf",
      },
      new()
      {
        Key = "site.homepage.resourceregistration",
        Value = @"###Registering a sample resource
A sample resource is any infrastructure that holds or can collect and distribute human samples and data (e.g. biobanks, bioresources, biorepositories, cohorts and clinical trials).
####For more information
We have developed some resources to help you register:
* [View our help pages](https://www.biobankinguk.org/directory/)
* [View our videos](https://youtu.be/7mnjyUwVuTA)",
      },
      new()
      {
        Key = "site.homepage.networkregistration",
        Value = @"###Registering a network
A network is a group of sample resources that have come together with some common objective or agreed standard. An example is the [Confederation of Cancer Biobanks](https://directory.biobankinguk.org/Profile/Network/2).
####Sample resource or Network?
If you are unsure about whether to register a resource or network then [please get in touch](https://www.biobankinguk.org/contact-us/).",
      },
      new()
      {
        Key = "site.homepage.searchradiosamplescollected",
        Value = "For a group to collect samples on your behalf",
        Description = "Set require samples collected text",
        ReadOnly = false,
      },
      new()
      {
        Key = "site.homepage.searchradioaccesssamples",
        Value = "Access existing samples",
        Description = "Set access existing samples text",
        ReadOnly = false,
      },

      #endregion

      #region Register Page

      new()
      {
        Key = "site.register.biobank.title",
        Value = "Register a new sample resource"
      },
      new()
      {
        Key = "site.register.biobank.description",
        Value =
          "If you have a collection of tissue samples or the ability to collect samples for researchers then you can add an overview to our database by registering below.",
      },
      new()
      {
        Key = "site.register.network.title",
        Value = "Register a new network"
      },
      new()
      {
        Key = "site.register.network.description",
        Value = "If you are a network, such as the Confederation of Cancer Biobanks, you can register below.",
      },
      new()
      {
        Key = "site.register.help.show",
        Value = "false",
      },
      new()
      {
        Key = "site.register.help.url",
        Value = "https://biobankinguk.org/share/",
      },

      #endregion

      #region SampleResource Page

      // Show Data Sharing 
      new()
      {
        Key = "site.sampleresource.datasharing",
        Value = "true",
        Name = "Use Data Sharing",
        Description = "Enable/Disable data sharing",
        ReadOnly = false
      },
      // Ethics Field Name
      new()
      {
        Key = "site.sampleresource.ethics.name",
        Value = "Ethics Committee Approval",
        Name = "Ethics Field Name",
        Description = "Set ethics field name",
        ReadOnly = false
      },
      // Sample Resource Name
      new()
      {
        Key = "site.sampleresource.name",
        Value = "Sample Resource",
        Name = "Sample Resource Name",
        Description = "Set sample resource name",
        ReadOnly = false
      },
      new()
      {
        Key = "site.sampleresource.ethics.type",
        Value = "true",
        Name = "Ethics Field Type",
        Description = "If set to true ethics field type is a checkbox, else free text",
        ReadOnly = false
      },

      #endregion

      // Enable/Disable Registration Emails
      new()
      {
        Key = "site.email.registration",
        Value = "true",
        Name = "Allow registration emails to be sent",
        Description = "Enable/Disable whether registration emails are to be sent",
        ReadOnly = false
      },
      // Term Page Info
      new()
      {
        Key = "site.termpage.pageinfo",
        Value =
          @"This page is designed to help you find sample resources using SNOMED CT terms. We are committed to using established standards. SNOMED CT is currently being rolled out across the NHS and therefore we have adopted these [terms for disease classifications](https://biobankinguk.org/snomed-ct-using/).
We do appreciate that not everyone will be aware of the terms used on the FIND Virtual Biobank Directory(VBD). The table below captures all the terms that sample resources have used in the Directory so far. Simply use the search to filter the list and then click on 'Find Biobanks'.",
      }
    };

    set.AddRange(defaults
      .Where(x => !set.Any(y => y.Key == x.Key)));

    await _db.SaveChangesAsync();
  }

  public async Task SeedOrganisationTypes()
  {
    var set = _db.Set<OrganisationType>();

    var organisationTypes = new List<OrganisationType>
    {
      new()
      {
        OrganisationTypeId = 1,
        Description = "Biobank",
        SortOrder = 1
      }
    };

    // Seed Missing Values
    set.AddRange(organisationTypes
      .Where(x => !set.Any(y => y.Description == x.Description)));

    await _db.SaveChangesAsync();
  }

  public async Task SeedContentPages()
  {
    var set = _db.Set<ContentPage>();

    var pages = new List<ContentPage>()
    {
      new()
      {
        Title = "About",
        Body = "",
        RouteSlug = "About",
        IsEnabled = true,
        LastUpdated = DateTime.UtcNow
      },
      new()
      {
        Title = "Privacy Policy",
        Body = "",
        RouteSlug = "Privacy",
        IsEnabled = true,
        LastUpdated = DateTime.UtcNow
      }
    };

    _db.ContentPages.AddRange(pages.Where(x => !set.Any(y => y.RouteSlug == x.RouteSlug)));

    await _db.SaveChangesAsync();
  }

  /// <summary>
  /// Ensure an individual role exists and has the specified claims
  /// </summary>
  /// <param name="roleName">The name of the role to ensure is present</param>
  /// <param name="claims">The claims the role should have</param>
  /// <returns></returns>
  private async Task SeedRole(string roleName, List<(string type, string value)> claims)
  {
    var role = await _roles.FindByNameAsync(roleName);

    // create the role if it doesn't exist
    if (role is null)
    {
      role = new IdentityRole { Name = roleName };
      await _roles.CreateAsync(role);
    }

    // ensure the role has the claims specified
    // turning this into a dictionary gives us key indexing, not needing to repeatedly enumerate the list
    var existingClaims = (await _roles.GetClaimsAsync(role)).ToDictionary(x => $"{x.Type}{x.Value}");
    foreach (var (type, value) in claims)
    {
      // only add the claim if the role doesn't already functionally have it
      if (!existingClaims.ContainsKey($"{type}{value}"))
        await _roles.AddClaimAsync(role, new Claim(type, value));
    }
  }

  public async Task SeedRoles()
  {
    var roles = new[] { Role.BiobankAdmin, Role.NetworkAdmin, Role.DirectoryAdmin, Role.SuperUser };
    
    foreach (var role in roles) await SeedRole(role, new());
  }
}
