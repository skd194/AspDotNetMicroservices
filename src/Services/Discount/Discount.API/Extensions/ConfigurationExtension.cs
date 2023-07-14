namespace Discount.API.Extensions
{
    public static class ConfigurationExtension
    {
        public static string GetDbConnectionString(this IConfiguration configuration)
            => configuration
                .GetValue<string>("DatabaseSettings:ConnectionString");
    }
}
