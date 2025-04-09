using FullAuth.Dto;
using FullAuth.Entities;
using FullAuth.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FullAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }


        /*************************************************************/
        /******************** GET ALL PRODUCT ****************************/
        /*************************************************************/
        [HttpGet]
        [Authorize(Roles = "ADMIN,USER")]
        public async Task<ActionResult<ResponseDto>> GetAll()
        {
            ResponseDto response = new ResponseDto();
            response.Data = await _productRepository.GetAllProducts();
            return response;
        }


        /*************************************************************/
        /******************** ADD PRODUCT ****************************/
        /*************************************************************/
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Add([FromBody] ProductDto req)
        {
            ResponseDto response = new ResponseDto();
            Product product = new Product()
            {
                ProductName = req.ProductName,
                Price = req.Price
            };

            bool result = await _productRepository.AddProduct(product);

            if (!result)
            {
                response.IsSuccess = false;
                response.Message = "Internal Error";
                return BadRequest(response);
            }

            response.Message = "Product Added successfully";

            return Ok(response);
        }


        /*************************************************************/
        /******************** DELETE PRODUCT *************************/
        /*************************************************************/
        [HttpDelete("{ProductId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ResponseDto>> Delete(int ProductId)
        {
            ResponseDto response = new ResponseDto();
            if (!await _productRepository.DeleteProductById(ProductId))
            {
                response.IsSuccess = false;
                response.Message = "Wrong Product Id";
            }
            return response;
        }

    }
}
