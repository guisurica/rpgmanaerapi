namespace rpgmanagerapi.Routes;

public static class UserRoutes
{
    public static void UserEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/users");


    }
}