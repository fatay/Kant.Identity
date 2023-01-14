using AutoMapper;
using Kant.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kant.Identity.Dependencies;

public class DependencyContainer
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public DependencyContainer(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }
}
