using AutoMapper;
using Kant.Identity.Entities;
using Kant.Identity.Models.ResultModels;

namespace Kant.Identity.Mapping;

public class IdentityMappingProfile : Profile
{
	public IdentityMappingProfile()
	{
        CreateMap<User, SignUpResultModel>().ReverseMap();
    }
}
