using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private UserContext _context;
        //private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;

            if (_context.Users.Count() == 0)
            {
                _context.Users.Add(new UserDTO { Name = "Merets", Age = 35, Location = "RA", Work = new WorkDTO() { Id = 1, Name = "Sela", Location = "BB", Rating = 5.0 } });
                _context.Users.Add(new UserDTO { Name = "Tomer", Age = 140, Location = "RA", Work = new WorkDTO() { Id = 2, Name = "Sela", Location = "BB", Rating = 5.0 } });
                _context.Users.Add(new UserDTO { Name = "Itzhak", Age = 20, Location = "RA", Work = new WorkDTO() { Id = 3, Name = "Sela", Location = "BB", Rating = 5.0 } });
                _context.Users.Add(new UserDTO { Name = "Yaakov", Age = 60, Location = "RA", Work = new WorkDTO() { Id = 4, Name = "Sela", Location = "BB", Rating = 5.0 } });
                _context.Users.Add(new UserDTO { Name = "David", Age = 40, Location = "RA", Work = new WorkDTO() { Id = 5, Name = "Sela", Location = "BB", Rating = 5.0 } });
                _context.Users.Add(new UserDTO { Name = "Avraham", Age = 100, Location = "LA", Work = new WorkDTO() { Id = 6, Name = "Sela", Location = "BB", Rating = 5 } });
                _context.Users.Add(new UserDTO { Name = "Shlomo", Age = 30, Location = "RA", Work = new WorkDTO() { Id = 7, Name = "Sela", Location = "BB", Rating = 5.0 } });
                _context.SaveChanges();
            }
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUser()
        {
            return await _context.Users.Include(u => u.Work).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            await RefreshContext();

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        private async Task RefreshContext()
        {
            var users = await _context.Users
                            .Include(u => u.Work)
                            .ToListAsync();
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDTO user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<string>> PostUser(UserDTO user)
        {
            var previousMaxId = _context.Users.Max(u => u.Id);
            var maxId = previousMaxId + 1;
            user.Id = maxId;
            user.Work.Id = maxId;

            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Workaround!
                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    return CreatedAtAction("GetUser", new { id = user.Id }, user);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserDTO>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }


        // PATCH: api/Users/5   // My addition
        [HttpPatch("{id}")]
        public async Task<ActionResult<string>> PatchUser(UserDTO user)
        {
            var userFromDB = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (userFromDB == null)
                return NotFound();
            else
            {
                if (string.IsNullOrEmpty(user.Name) == false)
                    userFromDB.Name = user.Name;
                if (user.Age != 0)
                    userFromDB.Age = user.Age;
                if (string.IsNullOrEmpty(user.Location) == false)
                    userFromDB.Location = user.Location;
                if (user.Work != null)
                    userFromDB.Work = user.Work;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Workaround!
                if (ex.Message.Contains("An item with the same key has already been added"))
                {
                    return NoContent();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

    }
}
