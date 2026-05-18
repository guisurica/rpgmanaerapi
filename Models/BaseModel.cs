namespace rpgmanagerapi.Models;

public abstract class BaseModel
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public DateTime CreatedOnUtc { get; protected set; }
    public DateTime? UpdatedOnUtc { get; protected set; }

    public List<string> Words { get; protected set; } = new();

    public BaseModel(string name)
    {
        Id = Guid.NewGuid();
        CreatedOnUtc = DateTime.UtcNow;

        Name = name;

        CreateFilterWords();
    }

    private void CreateFilterWords()
    {
        if (string.IsNullOrEmpty(Name))
        {
            return;
        }

        var splittedName = Name.Split(" ");

        foreach (var str in splittedName)
        {
            if (string.IsNullOrEmpty(str)) continue;

            var stringSanitazer = str.ToLower().Trim();
            Words.Add(stringSanitazer);
        }
    }
}