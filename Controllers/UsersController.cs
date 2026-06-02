using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]  // ✅ Ensures automatic model validation
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static List<User> users = new List<User>();

        // GET: api/users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(users);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound($"User with ID {id} not found.");
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User newUser)
        {
            // ✅ Validation check
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ✅ Prevent duplicates by email
            if (users.Any(u => u.Email == newUser.Email))
                return Conflict($"User with email {newUser.Email} already exists.");

            newUser.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(newUser);

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound($"User with ID {id} not found.");

            // ✅ Prevent duplicate email on update
            if (users.Any(u => u.Email == updatedUser.Email && u.Id != id))
                return Conflict($"Another user with email {updatedUser.Email} already exists.");

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Department = updatedUser.Department;

            return NoContent();
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound($"User with ID {id} not found.");

            users.Remove(user);
            return NoContent();
        }
    }
}
