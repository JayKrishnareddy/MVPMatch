using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using MVPMatch.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVPMatch.Controllers;
    
    [AllowAnonymous]
    public class AuthenticateController : BaseController
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly PasswordEncryption _passwordEncryption;
        public AuthenticateController(DataContext dataContext, IConfiguration configuration, PasswordEncryption passwordEncryption,IHttpContextAccessor httpContextAccessor) : base(configuration,httpContextAccessor)
        {
            _dataContext = dataContext;
            _configuration = configuration;
            _passwordEncryption = passwordEncryption;
            _httpContext = httpContextAccessor;
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
    }