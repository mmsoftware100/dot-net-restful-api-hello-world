using System.ComponentModel.DataAnnotations;

namespace TestAPI.Models
{
    public class WishBottle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [MaxLength(256)]
        public string Message { get; set; } = string.Empty;
        
        // Anonymous user tracking
        public string? UserFingerprint { get; set; }
        
        // Engagement metrics
        public int Hearts { get; set; } = 0;
        public int Smiles { get; set; } = 0;
        public int Stars { get; set; } = 0;
        
        // Status & lifecycle
        public BottleStatus Status { get; set; } = BottleStatus.Active;
        public DateTime ThrownAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(30);
        
        // For avoiding duplicate catches
        public ICollection<CatchRecord> CatchRecords { get; set; } = new List<CatchRecord>();
    }

    public enum BottleStatus
    {
        Active,
        Expired,
        Archived
    }
}