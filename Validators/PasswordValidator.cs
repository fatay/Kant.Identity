using Identite.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identite.Validators;

public class PasswordValidator : IPasswordValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> userManager, User user, string password)
    {
        var userNameLower = user.UserName.ToLower();
        var passwordLower = password.ToLower();

        var identityErrors = new List<IdentityError>();

        if (passwordLower.Contains(userNameLower))
        {
            identityErrors.Add(new()
            {
                Code = "PasswordContainsUserName",
                Description = "Password should not contain your username."
            });
        }

        if (!identityErrors.Any())
            return Task.FromResult(IdentityResult.Success);
        else
            return Task.FromResult(IdentityResult.Failed(identityErrors.ToArray()));

    }
}
