using FluentValidation;
using Identite.Models.ResultModels;

namespace Identite.Validators.FluentValidation;

public sealed class SignInValidator : AbstractValidator<SignInResultModel>
{
    public SignInValidator()
    {
        MailOrUserName(); Password();
    }

    public void MailOrUserName() => RuleFor(user => user.MailOrUserName).NotEmpty();
    public void Password() => RuleFor(user => user.Password).NotEmpty();
}
