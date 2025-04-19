using AutoMapper;
using easyshifthq.Invitations;

namespace easyshifthq;

public class easyshifthqApplicationAutoMapperProfile : Profile
{
    public easyshifthqApplicationAutoMapperProfile()
    {
        CreateMap<Invitation, InvitationDto>();
    }
}
