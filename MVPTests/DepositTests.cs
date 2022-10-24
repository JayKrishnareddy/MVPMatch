using Microsoft.EntityFrameworkCore;
using MVPMatch;
using MVPMatch.Models;

namespace MVPTests
{
    public class DepositTests
    {
        private readonly DepositController _controller;
        private readonly DataContext dataContext;

        public DepositTests()
        {
            dataContext = GetDatabaseContext();
            _controller = new DepositController(dataContext);
        }

        [Fact]
        public void Deposit_API_WhenCalled_ReturnsOkResult()
        {
            var okResult = _controller.DepositAmountInVendingMachine("Jay", 50);
            Assert.True(okResult.IsCompletedSuccessfully);
        }
        public DataContext GetDatabaseContext()
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
                    databaseContext.SaveChanges();
                }
            }
            return databaseContext;
        }
    }
}
