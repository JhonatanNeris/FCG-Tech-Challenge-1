using FCG.Domain.Enums;
using System.Data;

namespace FCG.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Role Role { get; private set; }

    // Relação
    public ICollection<LibraryItem> LibraryItems { get; private set; } = new List<LibraryItem>();

    protected User() { } // EF Core

    public User(string name, string email, string passwordHash, Role role)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void AddGameToLibrary(Game game)
    {
        // Regra de negócio: não duplicar
        if (!LibraryItems.Any(ug => ug.GameId == game.Id))
        {
            LibraryItems.Add(new LibraryItem(this.Id, game.Id));
        }
    }


}
