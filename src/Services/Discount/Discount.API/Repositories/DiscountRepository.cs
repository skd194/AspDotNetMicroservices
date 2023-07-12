using Dapper;
using Discount.API.Entities;
using Npgsql;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly string _connectionString;
        public DiscountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");
        }

        public async Task<Coupon> Get(string productName)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var coupon = await connection
                .QueryFirstOrDefaultAsync<Coupon>(
                @"SELECT * 
                    FROM Coupon
                    WHERE ProductName = @ProductName",
                new { ProductName = productName }
                );

            return coupon ?? Coupon.NoDiscount;
        }

        public async Task<bool> Create(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var affected = await connection.ExecuteAsync(
                $@"INSERT INTO Coupon 
                  (
                    ProductName, 
                    Description, 
                    Amount
                  )
                  VALUES
                  (
                    @{nameof(coupon.ProductName)}, 
                    @{nameof(coupon.Description)}, 
                    @{nameof(coupon.Amount)}
                  )      
                ",
                new
                {
                    coupon.ProductName,
                    coupon.Description,
                    coupon.Amount
                });

            return affected != 0;
        }

        public async Task<bool> Update(Coupon coupon)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var affected = await connection.ExecuteAsync(
                $@"UPDATE Coupon 
                    SET 
                        ProductName=@{nameof(coupon.ProductName)},
                        Description=@{nameof(coupon.Description)},
                        Amount=@{nameof(coupon.Amount)}
                    WHERE Id = @{nameof(coupon.Id)}
                 ",
                new
                {
                    coupon.ProductName,
                    coupon.Description,
                    coupon.Amount,
                    coupon.Id
                });

            return affected != 0;

        }

        public async Task<bool> Delete(string productName)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var affected = await connection
                .ExecuteAsync(
                $@"DELETE FROM Coupon 
                   WHERE ProductName=@{nameof(productName)}
                 ",
                new { productName });

            return affected != 0;

        }

    }
}
