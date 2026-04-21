namespace FCG.Domain.Entities;

public class LibraryItem
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid GameId { get; private set; }
    public Game Game { get; private set; } = null!;

    public DateTime AcquiredAt { get; private set; }

    protected LibraryItem() { } // EF Core

    public LibraryItem(Guid userId, Guid gameId)
    {
        UserId = userId;
        GameId = gameId;
        AcquiredAt = DateTime.UtcNow;
    }
}
