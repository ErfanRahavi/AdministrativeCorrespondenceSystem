using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdministrativeCorrespondenceSystem.Data;
using AdministrativeCorrespondenceSystem.Models;

namespace AdministrativeCorrespondenceSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonsController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            return await _context.Persons.ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        
        [HttpPost]
        public async Task<ActionResult<Person>> CreatePerson(Person person)
        {
            if (string.IsNullOrEmpty(person.Name) || string.IsNullOrEmpty(person.Email))
            {
                return BadRequest("Name and Email are required.");
            }

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, Person person)
        {
            if (id != person.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            if (string.IsNullOrEmpty(person.Name) || string.IsNullOrEmpty(person.Email))
            {
                return BadRequest("Name and Email are required.");
            }

            var existingPerson = await _context.Persons.FindAsync(id);
            if (existingPerson == null)
            {
                return NotFound();
            }

            existingPerson.Name = person.Name;
            existingPerson.Email = person.Email;

            _context.Entry(existingPerson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}
