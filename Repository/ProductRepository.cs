using FullAuth.Data;
using FullAuth.Entities;
using Microsoft.EntityFrameworkCore;

namespace FullAuth.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AuthContext _authContext;
        public ProductRepository(AuthContext authContext)
        {
            _authContext = authContext;
        }

        public async Task<bool> AddProduct(Product product)
        {
            _authContext.products.Add(product);
            return await _authContext.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DeleteProductById(int productId)
        {
            //return (await _authContext.products
            //             .Where(p => p.ProductId == productId)
            //             .ExecuteDeleteAsync()
            //       ) > 0 ? true : false;

            var product = await _authContext.products.FindAsync(productId);
            if (product == null)
                return false;

            _authContext.products.Remove(product);
            var changes = await _authContext.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _authContext.products.ToListAsync();
        }

    }
}
