using AutoMapper;
using easyshifthq.Availabilities;
using easyshifthq.Invitations;
using easyshifthq.Locations;

namespace easyshifthq;

public class easyshifthqApplicationAutoMapperProfile : Profile
{
    public easyshifthqApplicationAutoMapperProfile()
    {
        CreateMap<Invitation, InvitationDto>();
        CreateMap<Location, LocationDto>();
        CreateMap<CreateUpdateLocationDto, Location>();
        CreateMap<LocationDto, CreateUpdateLocationDto>();
        
        // Availability mappings with explicit TimeSpan handling
        CreateMap<Availability, AvailabilityDto>()
            .ForMember(dest => dest.StartTime,
                opts => opts.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime,
                opts => opts.MapFrom(src => src.EndTime));
                
        CreateMap<SubmitWeeklyAvailabilityDto, Availability>();
        CreateMap<SubmitTimeOffRequestDto, Availability>();
    }
}
