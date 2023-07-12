namespace Discount.API.Entities
{
    public class Coupon
    {
        internal static Coupon NoDiscount =>
            new Coupon
            {
                ProductName = "No Discount",
                Amount = 0,
                Description = "No Discount Description",
            };

        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}
