using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using rpgmanagerapi.Data;
using rpgmanagerapi.Data.Config;
using rpgmanagerapi.Data.DTOs;
using rpgmanagerapi.Models;
using rpgmanagerapi.Validations;

namespace rpgmanagerapi.Routes;

public static class AuthRoutes
{
    public static void AuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/auth");

        authGroup.AllowAnonymous();

        authGroup.MapPost("/register", async (
            ApplicationDbContext _context,
            [FromBody] CreateUserDTO input,
            IOptions<JWTConfig> config
        ) =>
        {
            var createUserValidation = new CreateUserValidation();
            var validateInput = createUserValidation.Validate(input);

            if (!validateInput.IsValid) return Results.BadRequest(validateInput.Errors.FirstOrDefault().ErrorMessage);

            if (await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == input.Email) != null)
                return Results.BadRequest("Email already in use");

            if (await _context.Set<User>().FirstOrDefaultAsync(u => u.Name == input.Username) != null)
                return Results.BadRequest("Username already in use");

            var newUser = User.Create(input.Username, input.Email, input.Password);

            var userEntity = await _context.Set<User>().AddAsync(newUser.Data);

            await _context.SaveChangesAsync();

            var token = userEntity.Entity.GenerateJwtToken(config.Value);

            var userDTO = new UserDTO(userEntity.Entity.Email,
            userEntity.Entity.Name,
            userEntity.Entity.Id.ToString(),
            token);

            return Results.Ok(userDTO);
        });

        authGroup.MapPost("/login", async (
            ApplicationDbContext _context,
            [FromBody] LoginUserDTO input,
            IOptions<JWTConfig> config
        ) =>
        {
            var createUserValidation = new LoginUserValidation();
            var validateInput = createUserValidation.Validate(input);

            if (!validateInput.IsValid) return Results.BadRequest(validateInput.Errors.FirstOrDefault().ErrorMessage);

            var userFound = await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == input.Email);
            if (userFound == null) return Results.NotFound();

            if (!userFound.VerifyPassword(input.Password))
                return Results.BadRequest();

            var token = userFound.GenerateJwtToken(config.Value);

            var userDTO = new UserDTO(userFound.Email,
            userFound.Name,
            userFound.Id.ToString(),
            token);

            return Results.Ok(userDTO);
        });
    }
}