using Microsoft.AspNetCore.Authorization;
using MVPMatch.ViewModels;

namespace MVPMatch.Controllers
{
    public class BuyController : BaseController
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IConfiguration _configuration;
        private string userName = string.Empty;
        public BuyController(DataContext dataContext, IHttpContextAccessor httpContext, IConfiguration configuration) : base(configuration, httpContext)
        {
            _dataContext = dataContext;
            _httpContext = httpContext;
            _configuration = configuration;
            userName = ExtractJWTTokenFromHeader();
        }
        [HttpPost]
        public async Task<IActionResult> Buy([Required][FromBody] BuyProductModel buyProductModel)
        {
            try
            {
                int Amount = 0; List<int> RemainingAmount = new();
                var userInfo = await _dataContext.Users.FirstOrDefaultAsync(c => c.UserName.Equals(userName));
                var products = await _dataContext.Products.Where(c=>c.isActive.Equals(true)).Select(c => c.ProductId).ToListAsync();
                if (userInfo.Role.Equals("Buyer") && products.Contains(buyProductModel.ProductId))
                {
                    var userAmount = await _dataContext.DepositAccounts.Where(c => c.UserId.Equals(userInfo.UserId) && c.isActive.Equals(true)).Select(c => c.Amount).ToListAsync();
                    var productPrice = await _dataContext.Products.Where(c => c.ProductName.Equals(buyProductModel.ProductId)).Select(c => c.Price).FirstOrDefaultAsync();
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
                                AmountofProducts = buyProductModel.AmountofProducts
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
        public async Task<IActionResult> Reset()
        {
            var userInfo = await _dataContext.Users.Where(c => c.UserName.Equals(userName)).FirstOrDefaultAsync();
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
