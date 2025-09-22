Here's a RESTful API design for the two basic operations in .NET with Entity Framework:

## 🏗️ API Architecture

### **1. Throw Wish Bottle**
**Endpoint:** `POST /api/bottles/throw`

**Request:**
```json
{
  "message": "Your wish message here (max 256 chars)",
  "userFingerprint": "auto-generated-browser-id" // Optional
}
```

**Response:**
```json
{
  "bottleId": "guid",
  "message": "Your message",
  "thrownAt": "2024-01-15T10:30:00Z",
  "message": "Bottle thrown into the ocean! 🌊"
}
```

### **2. Catch Wish Bottle** 
**Endpoint:** `GET /api/bottles/catch`

**Query Parameters (optional):**
- `exclude` - Comma-separated bottle IDs to avoid repeats
- `userFingerprint` - To exclude user's own bottles

**Response:**
```json
{
  "bottleId": "guid",
  "message": "Random wish message",
  "thrownAt": "2024-01-14T08:15:00Z",
  "reactions": {
    "hearts": 5,
    "smiles": 2
  }
}
```

**404 Response (no bottles available):**
```json
{
  "message": "The ocean is calm today... No bottles found. Try again later! 🌅"
}
```

## 🗄️ Entity Framework Models

### **WishBottle Entity**
```csharp
public class WishBottle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MaxLength(256)]
    public string Message { get; set; }
    
    // Anonymous user tracking
    public string? UserFingerprint { get; set; }
    
    // Engagement metrics
    public int Hearts { get; set; }
    public int Smiles { get; set; }
    public int Stars { get; set; }
    
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
```

### **Catch Record Entity** (Prevent repeat catches)
```csharp
public class CatchRecord
{
    public Guid Id { get; set; }
    public Guid BottleId { get; set; }
    public string UserFingerprint { get; set; } // Who caught it
    public DateTime CaughtAt { get; set; } = DateTime.UtcNow;
    
    public WishBottle Bottle { get; set; }
}
```

## 🎯 Business Logic Flow

### **Throw Bottle Flow:**
1. Validate message (length, content filters)
2. Create bottle with expiration (30 days)
3. Generate optimistic "bottle traveled X miles" message
4. Return success with bottle ID

### **Catch Bottle Flow:**
1. Get random active bottle (not expired, not user's own)
2. Exclude recently caught bottles for this user
3. Create catch record to prevent repeats
4. Return bottle with engagement stats
5. If no bottles: poetic "calm ocean" message

## 🔧 Advanced Features Consideration

### **Rate Limiting:**
- Throw: 5 bottles/hour per fingerprint
- Catch: 20 catches/hour per fingerprint

### **Content Safety:**
- Basic profanity filter
- Report mechanism for inappropriate content
- Auto-archive after X reports

### **Performance Optimizations:**
- Database indexing on `Status`, `ExpiresAt`, `UserFingerprint`
- Cached "available bottles count"
- Background job to clean expired bottles

This design maintains complete anonymity while providing engaging, magical experience with minimal friction!