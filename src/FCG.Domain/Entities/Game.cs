namespace FCG.Domain.Entities;

public class Game
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public bool IsActive { get; private set; }

    //Relação
    public ICollection<LibraryItem> LibraryItems { get; private set; } = new List<LibraryItem>();
    public ICollection<Promotion> Promotions { get; private set; } = new List<Promotion>();

    protected Game() { } // EF Core

    public Game(string title, string description, decimal price)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Price = price;
        IsActive = true;
    }

    public void Update(string title, string description, decimal price)
    {
        Title = title;
        Description = description;
        Price = price;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}