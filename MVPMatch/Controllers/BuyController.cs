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
                    var userAmount = await _dataContext.DepositAccounts.Where(c => c.UserId.Equals(userInfo.UserId)).Select(c => c.Amount).ToListAsync();
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
    }
}
