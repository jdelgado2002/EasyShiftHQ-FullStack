using AutoMapper;
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
    }
}
