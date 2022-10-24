using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;

namespace MVPMatch.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly DataContext _dataContext;
        int[] Coins = { 5, 10, 20, 50, 100 };
        public DepositController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpPost(nameof(DepositAmountInVendingMachine))]
        public async Task<ActionResult> DepositAmountInVendingMachine([Required] string UserName, [Required] int Amount)
        {
            var userInfo = await _dataContext.Users.Where(c => c.UserName.Equals(c.UserName)).FirstOrDefaultAsync();
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
