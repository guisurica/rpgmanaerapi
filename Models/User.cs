using Microsoft.AspNetCore.Identity;
using rpgmanagerapi.Common;

namespace rpgmanagerapi.Models;

public sealed class User : BaseModel
{
    public string Email { get; private set; }
    public string Password { get; private set; }

    private User() : base(string.Empty) { }

    private User(string username, string email, string password) : base(username)
    {
        Name = username;
        Email = email;
        Password = password;
    }

    public static Result<User> Create(string username, string email, string password)
    {
        IPasswordHasher<User> passwordHasher = new PasswordHasher<User>();

        var newUser = new User(username, email, password);

        var hashedPass = passwordHasher.HashPassword(newUser, password);

        newUser.Password = hashedPass;

        return Result<User>.Success("User created successfully", 200, newUser);
    }

    public bool VerifyPassword(string password)
    {
        IPasswordHasher<User> passwordHasher = new PasswordHasher<User>();

        var verified = passwordHasher.VerifyHashedPassword(this, this.Password, password);

        if (verified != PasswordVerificationResult.Success) return false;

        return true;
    }

}