using FullAuth.Entities;

namespace FullAuth.Repository
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllProducts();

        Task<bool> AddProduct(Product product);

        Task<bool> DeleteProductById(int productId);
    }
}
