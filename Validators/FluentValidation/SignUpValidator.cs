using FluentValidation;
using Identite.Models.ResultModels;

namespace Identite.Validators.FluentValidation;

public sealed class SignUpValidator : AbstractValidator<SignUpResultModel>
{
    public SignUpValidator()
    {
        UserName(); Password(); FirstName(); LastName(); PhoneNumber(); MailAddress(); 
    }

    public void UserName() => RuleFor(user => user.UserName).NotEmpty().MaximumLength(60);
    public void Password() => RuleFor(user => user.Password).NotEmpty().MaximumLength(60);
    public void FirstName() => RuleFor(user => user.FirstName).NotEmpty().MaximumLength(60);
    public void LastName() => RuleFor(user => user.LastName).NotEmpty().MaximumLength(60);
    public void PhoneNumber() => RuleFor(user => user.PhoneNumber).NotEmpty().MaximumLength(16);
    public void MailAddress() => RuleFor(user => user.MailAddress).EmailAddress();
}
