namespace FCG.Domain.Entities
{
    public class Promotion
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid GameId { get; private set; }
        public Game Game { get; private set; } = null!;
        public decimal DiscountPercentage { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsActive { get; private set; }

        protected Promotion() { } // EF Core

        public Promotion(string name, Guid gameId, decimal discountPercentage, DateTime startDate, DateTime endDate)
        {
            Id = Guid.NewGuid();
            Name = name;
            GameId = gameId;
            DiscountPercentage = discountPercentage;
            StartDate = startDate;
            EndDate = endDate;
            IsActive = true;
        }

        public decimal CalculateDiscountAmount(decimal originalPrice)
        {
            if (!IsActive || DateTime.UtcNow < StartDate || DateTime.UtcNow > EndDate)
                return 0;

            return originalPrice * (DiscountPercentage / 100);
        }

        public void Update(string name, decimal discountPercentage, DateTime startDate, DateTime endDate)
        {
            Name = name;
            DiscountPercentage = discountPercentage;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}
