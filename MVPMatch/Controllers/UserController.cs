using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace MVPMatch.Controllers
{
    public class UserController : BaseController
    {
        private readonly DataContext _context;
        private readonly PasswordEncryption _passwordEncryption;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _configuration;
        private string userName = string.Empty;

        public UserController(DataContext context, PasswordEncryption passwordEncryption, IHttpContextAccessor httpContext, IConfiguration configuration) : base(configuration, httpContext)
        {
            _context = context;
            _passwordEncryption = passwordEncryption;
            _httpContext = httpContext;
            _configuration = configuration;
            userName = ExtractJWTTokenFromHeader();
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
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
        [AllowAnonymous]
        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<OkObjectResult> PostUser(User user)
        {
            user.Password = _passwordEncryption.Encryptdata(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("User Details Saved!");
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
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
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
