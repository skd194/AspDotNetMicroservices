using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System.Xml.Linq;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        public async Task Create(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> Delete(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>
                .Filter
                .Eq(p => p.Id, id);

            var deleteResult = await _context.Products.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<IEnumerable<Product>> Get() =>
            await _context
                .Products
                .Find(p => true)
                .ToListAsync();

        public async Task<Product> Get(string id) =>
            await _context
            .Products
            .Find(p => p.Id == id)
            .SingleOrDefaultAsync();

        public async Task<IEnumerable<Product>> GetByCategory(string categoryName)
        {
            FilterDefinition<Product> filter = Builders<Product>
                .Filter
                .Eq(p => p.Category, categoryName);

            return await _context
                .Products
                .Find(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>
                .Filter
                .Eq(p => p.Name, name);

            return await _context
                .Products
                .Find(filter)
                .ToListAsync();
        }

        public async Task<bool> Update(Product product)
        {
            var updateResult = await _context
                .Products
                .ReplaceOneAsync(
                    filter: g => g.Id == product.Id, 
                    replacement: product);

            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
