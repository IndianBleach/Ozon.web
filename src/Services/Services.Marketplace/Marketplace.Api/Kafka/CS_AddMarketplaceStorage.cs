using AutoMapper;
using Common.Repositories;
using Confluent.Kafka;
using Hangfire;
using Marketplace.Data.Entities.Sellers;
using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus.Serdes;
using Ozon.Bus;
using Marketplace.Data.Entities.Storages;
using Ozon.Bus.DTOs.StorageService;
using Microsoft.IdentityModel.Tokens;
using Marketplace.Data.Entities.Address;
using Hangfire.Dashboard;
using Common.DataQueries;
using Newtonsoft.Json.Linq;
using System.Reactive;

namespace Marketplace.Api.Kafka
{
    public class CS_AddMarketplaceStorage
    {
        private readonly ILogger<CS_AddMarketplaceStorage> _logger;

        private readonly IMapper _mapper;

        private readonly IServiceRepository<MarketplaceProductStorage> _storageRepository;

        private readonly IServiceRepository<AddressPoint> _addrRepository;

        private readonly IConsumerFactory _consumerFactory;

        public CS_AddMarketplaceStorage(
            IConsumerFactory consumerFactory,
            IServiceRepository<AddressPoint> addrRepository,
            IServiceRepository<MarketplaceProductStorage> storageRepository,
            ILogger<CS_AddMarketplaceStorage> logger)
        {
            _consumerFactory = consumerFactory;
            _addrRepository = addrRepository;
            _storageRepository = storageRepository;

            _logger = logger;
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogCritical("[Start consuming]");

            CancellationToken token = new CancellationToken();

            ConsumerWrapper<string, AddStorageMessage>? consumer = _consumerFactory.GetSingle<string, AddStorageMessage>();

            if (consumer != null)
            {
                try
                {
                    var data = consumer.ObservableData(new string[] { "storage-marketplace.addMarketplaceStorage" });

                    data.Subscribe(message =>
                    {
                        _logger.LogInformation($"[{nameof(CS_AddMarketplaceStorage)}] msgs received: {message.CityAddr} {message.ExternalStorageId}");

                        if (message.ExternalStorageId.HasValue)
                        {
                            QueryResult<AddressPoint> addrQuery = _addrRepository.Create(new AddressPoint(
                                city: message.CityAddr,
                                street: message.StreetAddr,
                                buildingNumber: message.BuildingNumberAddr));

                            if (addrQuery.IsSuccessed)
                            {
                                QueryResult<MarketplaceProductStorage> storageQuery = _storageRepository.Create(
                                    new MarketplaceProductStorage(
                                        externalStorageId: message.ExternalStorageId.Value,
                                        storageAddrId: addrQuery.Value.Id,
                                        verified: false));

                                if (!storageQuery.IsSuccessed)
                                    _logger.LogCritical($"channel[storage-marketplace.addMarketplaceStorage] storage create fail: {storageQuery.StatusMessage}");
                            }
                            else _logger.LogCritical($"channel[storage-marketplace.addMarketplaceStorage] addr create fail: {addrQuery.StatusMessage}");

                            // testing
                            consumer.Commit();
                        }
                        else _logger.LogCritical($"msg delivery error, channel[storage-marketplace.addMarketplaceStorage], null-exId");
                    });

                    consumer.StartConsume(token);
                }
                catch (Exception exp)
                {
                    _logger.LogError("spec erorr: " + exp.Message);
                }
            }
        }
    }
}
