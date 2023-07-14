using Npgsql;

namespace Discount.API.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void MigrateDatabase<TContext>(this IServiceProvider provider, int retry = 0)
        {
            var retryForAvailability = retry;

            var configuration = provider.GetRequiredService<IConfiguration>();
            var logger = provider.GetRequiredService<ILogger<TContext>>();

            try
            {
                logger.LogInformation("Migrating Postgresql database");

                using var connection = new NpgsqlConnection(configuration.GetDbConnectionString());
                connection.Open();
                using var command = new NpgsqlCommand
                {
                    Connection = connection,
                };

                command
                    .DropCouponTableIfExists()
                    .CreateCouponTable()
                    .SeedCouponTable();

                logger.LogInformation("Migrated Postgresql database");

            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "An error occured while migrating the postgresql database");
            
                if(retryForAvailability < 50)
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    provider.MigrateDatabase<TContext>(retryForAvailability);
                }
            }
        }

        #region private methods
        private static NpgsqlCommand SeedCouponTable(this NpgsqlCommand command)
        {
            command.CommandText = @"
            INSERT INTO Coupon 
                (ProductName, Description, Amount)
            VALUES
                ('IPhone X', 'IPhone Discount', 150),
                ('Samsung 10', 'Samsung Discount', 100)
            ";
            command.ExecuteNonQuery();
            return command;
        }


        private static NpgsqlCommand CreateCouponTable(this NpgsqlCommand command)
        {
            command.CommandText = @"CREATE TABLE Coupon 
                (
                        Id SERIAL PRIMARY KEY,
                        ProductName VARCHAR(24) NOT NULL,
                        Description TEXT,
                        Amount INT
                )";
            command.ExecuteNonQuery();
            return command;
        }

        private static NpgsqlCommand DropCouponTableIfExists(this NpgsqlCommand command)
        {
            command.CommandText = "DROP TABLE IF EXISTS Coupon";
            command.ExecuteNonQuery();
            return command;
        }
        #endregion private methods ends
    }
}
