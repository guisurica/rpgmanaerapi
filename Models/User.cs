namespace rpgmanagerapi.Models;

public sealed class User : BaseModel
{
    public string Email { get; private set; }
    public string Password { get; private set; }

    private User() : base(string.Empty) { }

}