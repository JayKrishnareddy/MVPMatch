namespace MVPMatch.Controllers
{
    public class DepositController : BaseController
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _configuration;
        private string userName = string.Empty;
        int[] Coins = { 5, 10, 20, 50, 100 };
        public DepositController(DataContext dataContext, IHttpContextAccessor httpContext, IConfiguration configuration) : base(configuration, httpContext)
        {
            _dataContext = dataContext;
            _httpContext = httpContext;
            _configuration = configuration;
            userName = ExtractJWTTokenFromHeader();
        }
        [HttpPost(nameof(DepositAmountInVendingMachine))]
        public async Task<ActionResult> DepositAmountInVendingMachine([Required] int Amount)
        {
            var userInfo = await _dataContext.Users.Where(c => c.UserName.Equals(userName)).FirstOrDefaultAsync();
            if (Coins.Contains(Amount) && userInfo.Role.Equals("Buyer"))
            {
                var deposit = new DepositAccount
                {
                    Amount = Amount,
                    UserId = userInfo.UserId
                };
                _dataContext.DepositAccounts.Add(deposit);
                await _dataContext.SaveChangesAsync();
                return Ok("Amount Deposited in the Vending Machine!");
            }
            else
            {
                return BadRequest($"Amount not deposited, Users with only Buyer role and 5,10,20,5,100 can only be deposit");
            }
        }
    }
}
