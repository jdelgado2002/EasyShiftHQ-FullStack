using AutoMapper;
using easyshifthq.Invitations;
using easyshifthq.Web.Pages.Invitations;

namespace easyshifthq.Web.ObjectMapping;

public class easyshifthqWebAutoMapperProfile : Profile
{
    public easyshifthqWebAutoMapperProfile()
    {
        CreateMap<CreateModalModel.CreateInvitationViewModel, CreateInvitationDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId));
    }
}