using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using rpgmanagerapi.Common;
using rpgmanagerapi.Data.Config;

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

    public string GenerateJwtToken(JWTConfig config)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, this.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, this.Name),
            new Claim(JwtRegisteredClaimNames.Email, this.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "rpgmanager.com",
            audience: "rpgmanager.com",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}