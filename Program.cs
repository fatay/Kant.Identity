using AutoMapper;
using Kant.Identity.Context;
using Kant.Identity.Dependencies;
using Kant.Identity.Entities;
using Kant.Identity.Models;
using Kant.Identity.Models.ResultModels;
using Kant.Identity.Validators;
using Kant.Identity.Validators.FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

#region Services

// Add Logging
builder.Host.UseSerilog((hostContext, services, configuration) => {
        configuration.WriteTo.Seq("http://localhost:5341")
                     .Enrich.WithProperty("AppName", "Kant.Identity")
                     .Enrich.WithProperty("Environment", "development");
});

// Add services to the container.
var services = builder.Services;

// Add Endpoints to the project.
services.AddEndpointsApiExplorer();

// Add Swagger UI for development environment.
services.AddSwaggerGen();

// Add UserDbContext to project by using Postgres 
services.AddDbContext<UserDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("UserDbConnectionString")));

// Add App defaults to projects
services.Configure<AppSettingsModel>(builder.Configuration.GetSection("AppSettings"));

// Add Microsoft Identity for managing user operations
services.AddIdentity<User, Role>().AddEntityFrameworkStores<UserDbContext>()
                                  .AddPasswordValidator<PasswordValidator>()
                                  .AddUserValidator<UserValidator>()
                                  .AddDefaultTokenProviders();

// Identity Options
services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});

// Add Cors to project => Any Method, Any Header allowed for Identity Project
services.AddCors(p => p.AddPolicy("usercors", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

services.AddAuthentication();
services.AddAuthorization();

services.AddAutoMapper(Assembly.GetExecutingAssembly());
services.AddScoped<DependencyContainer>();

// Configure cookies
services.ConfigureApplicationCookie(cookieOption =>
{
    var cookieBuilder = new CookieBuilder
    {
        Name = "IdentityCookie",
    };

    cookieOption.Cookie = cookieBuilder;
    cookieOption.LoginPath = new PathString("/signin");
    cookieOption.LogoutPath = new PathString("/logout");
    cookieOption.ExpireTimeSpan = TimeSpan.FromDays(20);
    cookieOption.SlidingExpiration = true;
});

#endregion

#region App Settings 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("usercors");
app.UseAuthentication();
app.UseAuthorization();

#endregion

app.MapPost("/signup", async (UserManager<User> userManager, IMapper mapper, [FromBody] SignUpResultModel request) =>
{
    Log.Information("[SignUp][RequestStart] : {@Request}", request);

    #region Validations

    var validationWarnings = new List<KeyValuePair<string, string>>();

    var validationResult = new SignUpValidator().Validate(request);

    if (!validationResult.IsValid)
    {
        var fluentValidationWarnings = validationResult.Errors.Select(s => new KeyValuePair<string, string>(s.ErrorCode, s.ErrorMessage)).ToList();
        validationWarnings.AddRange(fluentValidationWarnings);
    }

    if (request.Password != request.PasswordConfirm)
    {
        var passwordValidationError = new KeyValuePair<string, string>("PasswordsDoNotMatch", "Passwords do not match.");
        validationWarnings.Add(passwordValidationError);
    }

    if (validationWarnings.Any())
    {
        Log.Warning("[SignUp][RequestEnd] : Request was terminated with validation warnings {@Warnings} {@Request}", validationWarnings, request);

        return new ServiceResultModel<User>(validationWarnings);
    }

    #endregion

    #region User Operations

    var user = mapper.Map<User>(request);

    var identityResult = await userManager.CreateAsync(user, request.Password);

    if (identityResult.Succeeded)
    {
        Log.Information("[SignUp][RequestEnd] : User created successfully {@UserName} {@Request}", request.UserName, request);

        return new ServiceResultModel<User>();
    }
    else
    {
        var identityErrors = identityResult.Errors.Select(s => new KeyValuePair<string, string>(s.Code, s.Description)).ToList();

        Log.Warning("[SignUp][RequestEnd] : Request was terminated with identity errors {@Errors} {@Request}", identityErrors, request);

        return new ServiceResultModel<User>(identityErrors);
    }

    #endregion


}).WithName("Sign Up");

app.MapPost("/signin", async (UserManager<User> userManager, SignInManager<User> signInManager, [FromBody] SignInResultModel request) =>
{
    Log.Information("[SignIn][RequestStart] : {@Request}", request);

    #region Validations

    var validationWarnings = new List<KeyValuePair<string, string>>();

    var validationResult = new SignInValidator().Validate(request);

    if (!validationResult.IsValid)
    {
        var fluentValidationErrors = validationResult.Errors.Select(s => new KeyValuePair<string, string>(s.ErrorCode, s.ErrorMessage)).ToList();
        validationWarnings.AddRange(fluentValidationErrors);
    }

    var findUserByMail = await userManager.FindByEmailAsync(request.MailOrUserName);
    var findUserByName = await userManager.FindByNameAsync(request.MailOrUserName);

    var user = findUserByMail ?? findUserByName;

    if (user == null)
    {
        var userNotFoundWarning = new KeyValuePair<string, string>("WrongUserNameOrPassword", "Wrong username or password.");
        validationWarnings.Add(userNotFoundWarning);
    }

    if (validationWarnings.Any())
    {
        Log.Warning("[SignIn][RequestEnd] : Request was terminated with validation warnings {@Warnings} {@Request}", validationWarnings, request);

        return new ServiceResultModel<User>(validationWarnings);
    }

    #endregion

    #region User Operations
    
    var identityResult = await signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);

    if (identityResult.Succeeded)
    {
        Log.Information("[SignIn][RequestEnd] : User signed in successfully {@UserName} {@Request}", request.MailOrUserName, request);

        return new ServiceResultModel<User>();
    }
    else
    {
        var identityErrors = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("WrongUserNameOrPassword", "Wrong username or password.") };

        Log.Warning("[SignUp][RequestEnd] : Request was terminated with identity errors {@Errors} {@Request}", identityErrors, request);

        return new ServiceResultModel<User>(identityErrors);
    }

    #endregion


}).WithName("Sign In");

app.MapPost("/resetpassword", async (IOptions<AppSettingsModel> options, UserManager<User> userManager, SignInManager<User> signInManager, [FromBody] string mailAddress) =>
{
    Log.Information("[ResetPassword][RequestStart] : {@Request}", mailAddress);

    #region Validations

    var validationWarnings = new List<KeyValuePair<string, string>>();

    var user = await userManager.FindByEmailAsync(mailAddress);

    if (user == null)
    {
        var userNotFoundWarning = new KeyValuePair<string, string>("WrongUserNameOrPassword", "Wrong username or password.");
        validationWarnings.Add(userNotFoundWarning);
    }

    if (validationWarnings.Any())
    {
        Log.Warning("[SignIn][RequestEnd] : Request was terminated with validation warnings {@Warnings} {@Request}", validationWarnings, mailAddress);

        return new ServiceResultModel<User>(validationWarnings);
    }

    #endregion

    #region User Operations

    var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);

    var appSettings = options.Value;
    var subject = appSettings.AppName + "| Reset Password";
    var link = appSettings.DomainName + "/updatePassword?userId=" + user.Id + "&token&=" + passwordResetToken;

    var body = $@" 
        <div style='text-align:left'> 
            <h3> Reset Password | </h3> <br />
            <b>To reset password, click link below;</b>
            <a href = '{link}'> Reset Password </a> 
        </div>
    ";

    var smtpClient = new SmtpClient();

    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
    smtpClient.UseDefaultCredentials = false;
    smtpClient.Port = 587;
    smtpClient.Credentials = new NetworkCredential(appSettings.MailAddress, appSettings.Password);
    smtpClient.EnableSsl = true;

    var mailMessage = new MailMessage { IsBodyHtml = true };
    mailMessage.To.Add(mailAddress);
    mailMessage.Subject = subject;
    mailMessage.Body = body;

    await smtpClient.SendMailAsync(mailMessage);

    Log.Information("[ResetPassword][RequestEnd] : {@Request}", mailAddress);

    return new ServiceResultModel<User>();

    #endregion
    

}).WithName("ResetPassword");

app.MapPost("/updatepassword", async (UserManager<User> userManager, [FromBody] UpdatePasswordModel request) =>
{
    Log.Information("[UpdatePassword][RequestStart] : {@Request}", request);

    #region Validations

    var validationWarnings = new List<KeyValuePair<string, string>>();

    var user = await userManager.FindByIdAsync(request.UserId);

    if (user == null)
    {
        var userNotFoundWarning = new KeyValuePair<string, string>("WrongUserNameOrPassword", "Wrong username or password.");
        validationWarnings.Add(userNotFoundWarning);
    }

    if (request.Password != request.PasswordConfirm)
    {
        var passwordValidationError = new KeyValuePair<string, string>("PasswordsDoNotMatch", "Passwords do not match.");
        validationWarnings.Add(passwordValidationError);
    }

    if (validationWarnings.Any())
    {
        Log.Warning("[SignIn][RequestEnd] : Request was terminated with validation warnings {@Warnings} {@Request}", validationWarnings, request);

        return new ServiceResultModel<User>(validationWarnings);
    }

    #endregion

    #region User Operations

    var identityResult = await userManager.ResetPasswordAsync(user, request.Token, request.Password);

    if (identityResult.Succeeded)
    {
        return new ServiceResultModel<User>();
    }
    else
    {
        var errors = identityResult.Errors.Select(s => new KeyValuePair<string, string>(s.Code, s.Description)).ToList();
        return new ServiceResultModel<User>(errors);
    }

    #endregion

}).WithName("Update Password");

app.MapPost("/logout", async (SignInManager<User> signInManager, [FromBody] SignInResultModel request) =>
{
    Log.Information("[Logout][RequestStart] : {@Request}", request);

    #region User Operations

    await signInManager.SignOutAsync();

    Log.Information("[Logout][RequestEnd] : {@Request}", request);

    #endregion

}).WithName("Logout");

app.Run();
