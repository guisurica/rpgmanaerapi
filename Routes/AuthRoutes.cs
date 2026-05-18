using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rpgmanagerapi.Data;
using rpgmanagerapi.Data.DTOs;
using rpgmanagerapi.Models;
using rpgmanagerapi.Validations;

namespace rpgmanagerapi.Routes;

public static class AuthRoutes
{
    public static void AuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup("/auth");

        authGroup.MapPost("/register", async (
            ApplicationDbContext _context,
            [FromBody] CreateUserDTO input
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

            await _context.Set<User>().AddAsync(newUser.Data);

            await _context.SaveChangesAsync();

            return Results.Ok();
        });

        authGroup.MapPost("/login", async (
            ApplicationDbContext _context,
            [FromBody] LoginUserDTO input
        ) =>
        {
            var createUserValidation = new LoginUserValidation();
            var validateInput = createUserValidation.Validate(input);

            if (!validateInput.IsValid) return Results.BadRequest(validateInput.Errors.FirstOrDefault().ErrorMessage);

            var userFound = await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == input.Email);
            if (userFound == null) return Results.NotFound();

            if (!userFound.VerifyPassword(input.Password))
                return Results.BadRequest();
            
            return Results.Ok();
        });
    }
}