using Microsoft.AspNetCore.Mvc;

namespace Kant.Identity.Models
{
    public class UpdatePasswordModel
    {
        public UpdatePasswordModel(string userId, string password, string passwordConfirm, string token)
        {
            UserId = userId;
            Password = password;
            PasswordConfirm = passwordConfirm;
            Token = token;
        }

        public string UserId { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string Token { get; set; }

    }
}
