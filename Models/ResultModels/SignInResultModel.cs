namespace Identite.Models.ResultModels;

public class SignInResultModel
{
	public SignInResultModel(string mailOrUserName, string password, bool rememberMe)
	{
        MailOrUserName = mailOrUserName;
		Password = password;
		RememberMe = rememberMe;
	}

	public string MailOrUserName { get; set; }

	public string Password { get; set; }

	public bool RememberMe { get; set; }
}
