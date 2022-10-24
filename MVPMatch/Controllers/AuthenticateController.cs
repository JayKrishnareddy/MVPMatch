using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MVPMatch.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MVPMatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        public AuthenticateController(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromQuery] Login model)
        {
            var user = await _dataContext.Users.Where(c => c.UserName.Equals(model.Username) && c.Password.Equals(model.Password)).FirstOrDefaultAsync();
            if (user != null)
            {
                string name = HttpContext.Session.GetString("UserName");
                if (name is not null && name.Equals(user.UserName))
                {
                    return BadRequest("There is already an active session using your account,terminate all the active sessions on their account via an endpoint i.e. /logout");
                }
                else
                {
                    HttpContext.Session.SetString("UserName", model.Username);
                    var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                    authClaims.Add(new Claim(ClaimTypes.Role, user.Role));
                    var token = GetToken(authClaims);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }

            }
            return Unauthorized("Please pass valid username and password!");
        }

        [HttpPost(nameof(Logout))]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return Ok("All sessions has been terminated");
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
