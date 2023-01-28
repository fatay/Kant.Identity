using AutoMapper;
using Identite.Entities;
using Identite.Models.ResultModels;

namespace Identite.Mapping;

public class IdentityMappingProfile : Profile
{
	public IdentityMappingProfile()
	{
        CreateMap<User, SignUpResultModel>().ReverseMap();
    }
}
