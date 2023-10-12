using Common.DTOs.Catalog;
using Common.DTOs.Storage;
using Grpc.Net.Client;
using Grpc.Protos.Storages;
using Simulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Simulator.Streams
{
    internal class StorageServiceStream
    {
        private List<StorageRead> _storages;

        private List<CatalogProductShortRead> _marketplaceProducts;

        private GrpcChannel _toStorageChannel;

        private Utils _utils = new Utils();

        public StorageServiceStream()
        {
            _toStorageChannel = GrpcChannel.ForAddress("http://localhost:5009");

            Task.Run(async () =>
            {
                _storages = await _utils.GetDataAsync<List<StorageRead>>("http://localhost:9095/storages");
                _marketplaceProducts = await _utils.GetDataAsync<List<CatalogProductShortRead>>("http://localhost:9095/catalogs/products");

            }).Wait();
        }

        public async Task Stream_MarketProductsStorageRegistrations(int iterations)
        {
            var client = new StorageGrpcService.StorageGrpcServiceClient(_toStorageChannel);

            var stream = client.AddProductsToStorage();

            while (iterations-- > 0)
            {
                await stream.RequestStream.WriteAsync(new AddProductToStorageRequest
                {
                    MarketplaceProductId = _marketplaceProducts[_utils.Rand(0, _marketplaceProducts.Count)].MarketplaceProductId,
                    StorageId = 1,
                });

                Console.WriteLine(iterations);
            }

            await stream.RequestStream.CompleteAsync();

            var r = await stream.ResponseAsync;

            Console.WriteLine("end: " + r.SuccessValueId);


        }
    }
}
