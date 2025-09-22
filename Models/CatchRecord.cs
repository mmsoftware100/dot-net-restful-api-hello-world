namespace TestAPI.Models
{
    public class CatchRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BottleId { get; set; }
        public string UserFingerprint { get; set; } = string.Empty;
        public DateTime CaughtAt { get; set; } = DateTime.UtcNow;
        
        public WishBottle Bottle { get; set; } = null!;
    }
}