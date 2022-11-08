using Microsoft.IdentityModel.Tokens;
using MVPMatch.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVPMatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly PasswordEncryption _passwordEncryption;
        public AuthenticateController(DataContext dataContext, IConfiguration configuration, PasswordEncryption passwordEncryption)
        {
            _dataContext = dataContext;
            _configuration = configuration;
            _passwordEncryption = passwordEncryption;
        }
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromQuery] Login model)
        {
            model.Password = _passwordEncryption.Encryptdata(model.Password);
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
                    var token = GetToken(model.Username);

                    return Ok(token);
                }

            }
            return Unauthorized("Please pass valid username and password!");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(Logout))]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return Ok("All sessions has been terminated");
        }
        private string GetToken(string userId)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var tokenDescripter = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };
            var token = tokenhandler.CreateToken(tokenDescripter);
            return tokenhandler.WriteToken(token);
        }

        private string ValidateToken(string token)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            try
            {
                tokenhandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedtoken);
                var jwtToken = (JwtSecurityToken)validatedtoken;
                var userId = jwtToken.Claims.First(c => c.Type == "id").Value;

                return userId;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}