using Common.DataQueries;
using Common.Grpc.Extensions;
using Common.Repositories;
using GlobalGrpc;
using Grpc.Core;
using Grpc.Protos.Products;
using Products.Data.Entities;
using Products.Grpc.GrpcUtils;

namespace Products.Grpc.Services
{
    public class ProductService : ProductServiceGrpc.ProductServiceGrpcBase
    {
        private readonly IServiceRepository<Product> _productRepository;

        private readonly IServiceRepository<ProductSeller> _sellerRepository;

        public ProductService(
            IServiceRepository<Product> productRepository,
            IServiceRepository<ProductSeller> sellerRepository)
        {
            _productRepository = productRepository;
            _sellerRepository = sellerRepository;
        }

        public override async Task<QueryStringIdResult> CreateProductSeller(CreateProductSellerRequest request, ServerCallContext context)
        {
            ProductSeller seller = new ProductSeller(
                name: request.Title,
                bankAccountNumber: request.BankAccountNumber,
                site: request.ContactSite,
                contactEmail: request.ContactEmail,
                specialNumber: request.SpecialCode,
                description: request.Description,
                dateCreated: DateTime.Now);

            QueryResult<ProductSeller> query = _sellerRepository.Create(seller);

            return query.FromQueryResult();
        }

        public override async Task<QueryStringIdResult> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            bool sellerExists = _sellerRepository.Any(
                x => x.Id == request.SellerId);

            if (!sellerExists)
                return GrpcGlobalTools.Failure("seller doesn't exists");

            Product product = new Product(
                title: request.DefaultTitle,
                description: request.DefaultDescription,
                defaultPrice: request.DefaultPrice,
                dateCreated: DateTime.Now,
                dateUpdated: DateTime.Now,
                sellerId: request.SellerId);

            QueryResult<Product> prodResult = _productRepository.Create(product);

            return prodResult.FromQueryResult();
        }
    }
}
