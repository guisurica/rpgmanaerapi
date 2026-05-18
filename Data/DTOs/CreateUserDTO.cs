namespace rpgmanagerapi.Data.DTOs;

public record CreateUserDTO(string Username, string Email, string Password, string CPassword);