using Grpc.Net.Client;
using Grpc.Protos.Products;
using Microsoft.AspNetCore.Mvc;

namespace Ozon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly GrpcChannel _productsChannel;

        public ProductsController(IConfiguration config)
        {
            Console.WriteLine("[CTOR (products)] " + DateTime.Now);
        }

        [HttpPost("/[controller]/sellers")]
        public async Task<IActionResult> CreateProductSeller(
            string bank_number,
            string contact_mail,
            string contact_site,
            string desc,
            string title)
        {
            var productsClient = new ProductServiceGrpc.ProductServiceGrpcClient(_productsChannel);

            var res = productsClient.CreateProductSeller(new CreateProductSellerRequest
            { 
                BankAccountNumber = bank_number,
                ContactEmail = contact_mail,
                ContactSite = contact_site,
                Description = desc,
                SpecialCode = bank_number,
                Title = title
            });

            return Ok(res);
        }

        [HttpPost("/[controller]/")]
        public async Task<IActionResult> CreateProduct(
            string seller_id,
            string desc,
            double price,
            string title)
        {
            var productsClient = new ProductServiceGrpc.ProductServiceGrpcClient(_productsChannel);

            var res = productsClient.CreateProductAsync(new CreateProductRequest
            { 
                DefaultPrice = price,
                SellerId = seller_id,
                DefaultDescription = desc,
                DefaultTitle = title
            });

            return Ok(res);
        }


        [HttpGet("/[controller]/all")]
        public async Task<IActionResult> GetAllProducts()
        {
            var productsClient = new ProductServiceGrpc.ProductServiceGrpcClient(_productsChannel);

            var res = productsClient.GetAllProducts(new GetAllProductsRequest());

            return Ok(res);
        }
    }
}
