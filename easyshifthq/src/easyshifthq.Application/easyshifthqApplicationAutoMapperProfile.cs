using AutoMapper;
using easyshifthq.Availabilities;
using easyshifthq.Invitations;
using easyshifthq.Locations;

namespace easyshifthq;

public class EasyshifthqApplicationAutoMapperProfile : Profile
{
    public EasyshifthqApplicationAutoMapperProfile()
    {
        CreateMap<Invitation, InvitationDto>();
        CreateMap<Location, LocationDto>();
        CreateMap<CreateUpdateLocationDto, Location>();
        CreateMap<LocationDto, CreateUpdateLocationDto>();
        
        // Availability mappings with TimeSpan handling
        CreateMap<Availability, AvailabilityDto>()
            .ForMember(dest => dest.StartTime, 
                opts => opts.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime,
                opts => opts.MapFrom(src => src.EndTime));
                
        CreateMap<SubmitWeeklyAvailabilityDto, Availability>()
            .ForMember(dest => dest.StartTime,
                opts => opts.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime,
                opts => opts.MapFrom(src => src.EndTime));
                
        CreateMap<SubmitTimeOffRequestDto, Availability>();
    }
}
