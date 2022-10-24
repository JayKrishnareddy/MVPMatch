using MVPMatch.ViewModels;

namespace MVPMatch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public BuyController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpPost]
        public async Task<IActionResult> Buy([Required][FromBody] BuyProductModel buyProductModel)
        {
            try
            {
                int Amount = 0; List<int> RemainingAmount = new();
                var userInfo = await _dataContext.Users.FirstOrDefaultAsync(c => c.UserName.Equals(buyProductModel.UserName));
                var products = await _dataContext.Products.Select(c => c.ProductName).ToListAsync();
                if (userInfo.Role.Equals("Buyer") && products.Contains(buyProductModel.ProductName))
                {
                    var userAmount = await _dataContext.DepositAccounts.Where(c => c.UserId.Equals(userInfo.UserId) && c.isActive.Equals(true)).Select(c => c.Amount).ToListAsync();
                    var productPrice = await _dataContext.Products.Where(c => c.ProductName.Equals(buyProductModel.ProductName)).Select(c => c.Price).FirstOrDefaultAsync();
                    foreach (int i in userAmount)
                    {
                        if(productPrice > Amount)
                        {
                            Amount += i;
                        }
                        else
                        {
                            RemainingAmount.Add(i);
                        }
                        
                    }
                    if (Amount >= productPrice)
                    {
                        return Ok(new
                        {
                            Spent = productPrice,
                            ProductsPurchased = new BuyProductModel
                            {
                                ProductId = buyProductModel.ProductId,
                                ProductName = buyProductModel.ProductName
                            },
                            RemainingCents = RemainingAmount.ToArray()
                        });
                    }
                    else return BadRequest("User doesn't have sufficient amount");
                }
                else return BadRequest("User with Buyer Role can able to buy the product");
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        [HttpDelete]
        public async Task<IActionResult> Reset([Required] string UserName)
        {
            var userInfo = await _dataContext.Users.Where(c => c.UserName.Equals(UserName)).FirstOrDefaultAsync();
            if (userInfo.Role.Equals("Buyer"))
            {
                var depositAccounts = await _dataContext.DepositAccounts.Where(c => c.UserId.Equals(userInfo.UserId)).ToListAsync();
                foreach (var i in depositAccounts)
                {
                    i.isActive = false;
                    await _dataContext.SaveChangesAsync();
                }
                return Ok("Account Reset Done !");
            }
            else
            {
                return BadRequest("User with Buyer role can only be able to reset the account");
            }
        }
    }
}
