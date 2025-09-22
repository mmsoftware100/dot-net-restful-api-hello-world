using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BottlesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BottlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("throw")]
        public async Task<IActionResult> ThrowBottle([FromBody] ThrowBottleRequest request)
        {
            // Validate message
            if (string.IsNullOrWhiteSpace(request.Message) || request.Message.Length > 256)
            {
                return BadRequest(new { message = "Message is required and must be 256 characters or less." });
            }

            // Create new bottle
            var bottle = new WishBottle
            {
                Message = request.Message.Trim(),
                UserFingerprint = request.UserFingerprint,
                ThrownAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };

            _context.WishBottles.Add(bottle);
            await _context.SaveChangesAsync();

            return Ok(new ThrowBottleResponse
            {
                BottleId = bottle.Id,
                Message = bottle.Message,
                ThrownAt = bottle.ThrownAt,
                ResponseMessage = "Bottle thrown into the ocean! 🌊"
            });
        }

        [HttpGet("catch")]
        public async Task<IActionResult> CatchBottle([FromQuery] string? userFingerprint)
        {
            // Get bottles that are not expired and not user's own
            var query = _context.WishBottles
                .Where(b => b.Status == BottleStatus.Active && 
                           b.ExpiresAt > DateTime.UtcNow);

            // Exclude user's own bottles if fingerprint provided
            if (!string.IsNullOrEmpty(userFingerprint))
            {
                query = query.Where(b => b.UserFingerprint != userFingerprint);
                
                // Automatically exclude bottles already caught by this user
                var caughtBottleIds = await _context.CatchRecords
                    .Where(cr => cr.UserFingerprint == userFingerprint)
                    .Select(cr => cr.BottleId)
                    .ToListAsync();
                
                if (caughtBottleIds.Any())
                {
                    query = query.Where(b => !caughtBottleIds.Contains(b.Id));
                }
            }

            // Get random bottle
            var bottles = await query.ToListAsync();
            
            if (!bottles.Any())
            {
                return NotFound(new { message = "The ocean is calm today... No bottles found. Try again later! 🌅" });
            }

            var random = new Random();
            var randomBottle = bottles[random.Next(bottles.Count)];

            // Create catch record if user fingerprint provided
            if (!string.IsNullOrEmpty(userFingerprint))
            {
                var catchRecord = new CatchRecord
                {
                    BottleId = randomBottle.Id,
                    UserFingerprint = userFingerprint,
                    CaughtAt = DateTime.UtcNow
                };
                _context.CatchRecords.Add(catchRecord);
                await _context.SaveChangesAsync();
            }

            return Ok(new CatchBottleResponse
            {
                BottleId = randomBottle.Id,
                Message = randomBottle.Message,
                ThrownAt = randomBottle.ThrownAt,
                Reactions = new Reactions
                {
                    Hearts = randomBottle.Hearts,
                    Smiles = randomBottle.Smiles
                }
            });
        }
    }

    // Request/Response DTOs
    public class ThrowBottleRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? UserFingerprint { get; set; }
    }

    public class ThrowBottleResponse
    {
        public Guid BottleId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ThrownAt { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;
    }

    public class CatchBottleResponse
    {
        public Guid BottleId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ThrownAt { get; set; }
        public Reactions Reactions { get; set; } = new Reactions();
    }

    public class Reactions
    {
        public int Hearts { get; set; }
        public int Smiles { get; set; }
    }
}