using AutoMapper;
using Biobanks.Analytics.Dto;
using Biobanks.Submissions.Api.Areas.Biobank.Models;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class BiobankAnalyticReportProfile : Profile
{
  public BiobankAnalyticReportProfile()
  {
    CreateMap<OrganisationReportDto, BiobankAnalyticReport>();
  }
}

public class ProfileStatusProfile : Profile
{
  public ProfileStatusProfile()
  {
    CreateMap<ProfileStatusDTO, ProfileStatus>();
  }
}
public class ProfilePageViewsProfile : Profile
{
  public ProfilePageViewsProfile()
  {
    CreateMap<ProfilePageViewsDto,ProfilePageViews>();
  }
}
public class SearchActivityProfile : Profile
{
  public SearchActivityProfile()
  {
    CreateMap<SearchActivityDto, SearchActivity>();
  }
}

public class ContactRequestsProfile : Profile
{
  public ContactRequestsProfile()
  {
    CreateMap<ContactRequestsDto, ContactRequests>();
  }
}
public class QuarterlyCountsProfile : Profile
{
  public QuarterlyCountsProfile()
  {
    CreateMap<QuarterlyCountsDto, QuarterlyCounts>();
  }
}
