using MVPMatch.ViewModels;

namespace MVPMatch.Controllers
{
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public ProductsController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet(nameof(GetProducts))]
        public async Task<IActionResult> GetProducts() => Ok(await _dataContext.Products.Where(c => c.isActive.Equals(true)).ToListAsync());

        [HttpPost(nameof(CreateProduct))]
        public async Task<IActionResult> CreateProduct([FromBody] ProductModel productModel)
        {
            var userData = await GetUserInfo(productModel.UserName);
            if (userData is not null)
            {
                var product = new Products
                {
                    ProductName = productModel.ProductName,
                    Price = productModel.Price,
                    CreatedBy = userData.UserId,
                    CreatedDate = DateTime.Now,
                    isActive = true

                };
                _dataContext.Products.Add(product);
                await _dataContext.SaveChangesAsync();
                return Ok("Product Created");
            }
            else return BadRequest("User with Seller Role can only be able to create product.. Please contact Administrator");

        }
        [HttpPut(nameof(UpdateProduct))]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductModel productModel, [Required] string userName)
        {
            var userData = await GetUserInfo(userName);
            if (userData is not null)
            {
                var productinfo = await _dataContext.Products.Where(c => c.ProductName.Equals(productModel.ProductName) && c.CreatedBy.Equals(userData.UserId)).FirstOrDefaultAsync();
                productinfo.ProductName = productModel.ProductName;
                productinfo.Price = productModel.Price;
                productinfo.CreatedDate = DateTime.Now;
                await _dataContext.SaveChangesAsync();
                return Ok("Product Updated");
            }
            else return BadRequest("User with Seller Role can only be able to update product.. Please contact Administrator");

        }
        [HttpDelete(nameof(DeleteProduct))]
        public async Task<IActionResult> DeleteProduct([Required] string ProductName, [Required] string userName)
        {
            var userData = await GetUserInfo(userName);
            if (userData is not null)
            {
                var product = await _dataContext.Products.Where(c => c.ProductName.Equals(ProductName)).FirstOrDefaultAsync();
                product.isActive = false;
                await _dataContext.SaveChangesAsync();
                return NoContent();
            }
            else return NotFound("User with Seller Role can only be able to Delete product.. Please contact Administrator");
        }

        private async Task<User> GetUserInfo(string userName)
        {
            var userInfo = await _dataContext.Users.Where(c => c.UserName.Equals(userName)).FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(userInfo.Role) && userInfo.Role.Equals("Seller"))
            {
                return userInfo;
            }
            else
            {
                return null;
            }
        }
    }
}