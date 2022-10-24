namespace MVPMatch.ViewModels
{
    public class ProductModel
    {
        public string ProductName { get; set; }
        public long Price { get; set; }
        public string UserName { get; set; }
    }

    public class BuyProductModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string UserName { get; set; }
    }
}
