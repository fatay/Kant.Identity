namespace Kant.Identity.Models.ResultModels;

public class SignUpResultModel
{
    public SignUpResultModel(string userName, string firstName, string lastName, string mailAddress, 
                           string phoneNumber, string password, string passwordConfirm)
    {
        UserName = userName;
        FirstName = firstName;
        LastName = lastName;
        MailAddress = mailAddress;
        PhoneNumber = phoneNumber;
        Password = password;
        PasswordConfirm = passwordConfirm;
    }

    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string MailAddress { get; set; }

    public string PhoneNumber { get; set; }

    public string Password { get; set; }

    public string PasswordConfirm { get; set; }

}
