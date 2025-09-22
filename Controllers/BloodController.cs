using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestAPI.Controllers
{
    [Route("api/bloods")]
    [ApiController]
    public class BloodController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BloodController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<BloodController>
        [HttpGet]
        public IEnumerable<Blood> Get()
        {
            return _context.BloodGroup.ToList();
        }

        // GET api/<BloodController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blood>> Get(int id)
        {
            var blood = await _context.BloodGroup.FindAsync(id);

            if (blood == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(blood);
            }
        }

        // POST api/<BloodController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Blood newblood)
        {
            if (newblood == null)
            {
                return BadRequest();
            }
            _context.BloodGroup.Add(newblood);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = newblood.Id }, newblood);
        }

        // PUT api/<BloodController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Blood blood)
        {
            var updatedblood = await _context.BloodGroup.FindAsync(id);
            if (updatedblood == null)
            {
                return NotFound();
            }
            _context.Entry(blood).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error updating user.");
            }

            return NoContent();
        }

        // DELETE api/<BloodController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedblood = _context.BloodGroup.Find(id);
            if (deletedblood == null)
            {
                return NotFound();
            }
            _context.BloodGroup.Remove(deletedblood);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
