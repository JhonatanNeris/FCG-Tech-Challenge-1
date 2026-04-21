namespace FCG.Domain.Entities;

public class Game
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }

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

