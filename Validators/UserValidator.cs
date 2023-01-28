using Identite.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identite.Validators;

public class UserValidator : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> userManager, User user)
    {
        var isUserNameStartsWithNumeric = int.TryParse(user.UserName[0].ToString(), out _);

        var identityErrors = new List<IdentityError>();
        if (isUserNameStartsWithNumeric)
        {
            identityErrors.Add(new IdentityError()
            {
                 Code = "UserNameStartsWithNumeric",
                 Description = "UserName shouldn't start with numeric character"
            });
        }

        if (!identityErrors.Any())
            return Task.FromResult(IdentityResult.Success);
        else
            return Task.FromResult(IdentityResult.Failed(identityErrors.ToArray()));

    }
}
