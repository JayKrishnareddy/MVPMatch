
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVPMatch;
using MVPMatch.Models;
using MVPMatch.ViewModels;

namespace MVPTests
{
    public class BuyTests
    {
        private readonly BuyController _controller;
        private readonly DataContext dataContext;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;

        public BuyTests()
        {
            dataContext = GetDatabaseContext();
            _controller = new BuyController(dataContext,_httpContext,_configuration);
        }
        [Fact]
        public void Buy_API_WhenCalled_ReturnsOkResult()
        {
            var buyModel = new BuyProductModel
            {
                ProductId = 1,
                ProductName = "Mobile",
            };
            var okResult = _controller.Buy(buyModel);
            Assert.True(okResult.IsCompletedSuccessfully);
        }
        private DataContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();
            if (databaseContext.Users.Count() <= 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    databaseContext.Users.Add(new User()
                    {
                        UserId = i,
                        UserName = "Jay",
                        isActive = true,
                        Password = "test",
                        Role = "Buyer"
                    });
                }
                databaseContext.Products.Add(new Products()
                {
                    ProductId = 1,
                    ProductName = "Mobile",
                    Price = 35,
                    CreatedBy = 1,
                    isActive = true
                }) ;

                    databaseContext.DepositAccounts.Add(new DepositAccount()
                    {
                        DepositId = 1,
                        Amount = 5,
                        isActive = true,
                        UserId = 1
                    });
                databaseContext.DepositAccounts.Add(new DepositAccount()
                {
                    DepositId = 2,
                    Amount = 10,
                    isActive = true,
                    UserId = 1
                });
                databaseContext.DepositAccounts.Add(new DepositAccount()
                {
                    DepositId = 3,
                    Amount = 20,
                    isActive = true,
                    UserId = 1
                });
                databaseContext.DepositAccounts.Add(new DepositAccount()
                {
                    DepositId = 4,
                    Amount = 50,
                    isActive = true,
                    UserId = 1
                });
                databaseContext.DepositAccounts.Add(new DepositAccount()
                {
                    DepositId = 5,
                    Amount = 50,
                    isActive = true,
                    UserId = 1
                });
                databaseContext.DepositAccounts.Add(new DepositAccount()
                {
                    DepositId = 6,
                    Amount = 100,
                    isActive = true,
                    UserId = 1
                });

                databaseContext.SaveChanges();
            }
            return databaseContext;
        }
    }
}
