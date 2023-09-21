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

namespace Marketplace.Api.Kafka
{
    public class CS_AddMarketplaceStorage
    {
        private readonly ILogger<CS_AddMarketplaceStorage> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IServiceRepository<MarketplaceProductStorage> _storageRepository;

        private readonly IServiceRepository<AddressPoint> _addrRepository;

        public CS_AddMarketplaceStorage(
            IServiceRepository<AddressPoint> addrRepository,
            IServiceRepository<MarketplaceProductStorage> storageRepository,
            ILogger<CS_AddMarketplaceStorage> logger)
        {
            _addrRepository = addrRepository;
            _storageRepository = storageRepository;

            _logger = logger;

            _kafkaServer = "kafka-broker:9092";
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogCritical("[Start consuming]");

            CancellationTokenSource token = new CancellationTokenSource();

            try
            {
                Consumer<string, AddStorageMessage> consumer = new Consumer<string, AddStorageMessage>(
                    config: new ConsumerConfig()
                    {
                        GroupId = nameof(CS_AddMarketplaceStorage),
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        BootstrapServers = _kafkaServer,
                        EnableAutoCommit = false,
                        AutoCommitIntervalMs = 0,
                    },
                    valueDeserializator: new ServiceBusValueDeserializer<AddStorageMessage>(),
                    onExeption: (sender, exp) => {
                        Console.WriteLine("[CANCEL TOKEN]");
                        //consumer.Close();
                        token.Cancel();
                    });

                var observer = consumer.ConsumeObserv(new List<string> { "storage-marketplace.addMarketplaceStorage" });

                // update marketplace product storages stock info
                observer.Subscribe(
                    message =>
                    {
                        _logger.LogInformation($"[{nameof(CS_AddMarketplaceStorage)}] msgs received: {message.CityAddr} {message.ExternalStorageId}");

                        if (message.ExternalStorageId.HasValue)
                        {
                            //var transaction = _addrRepository.NewTransaction();

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

                                if (storageQuery.IsSuccessed)
                                {
                                    _logger.LogInformation("channel[storage-marketplace.addMarketplaceStorage] storage created");
                                }
                                else
                                {
                                    _logger.LogCritical($"channel[storage-marketplace.addMarketplaceStorage] storage create fail: {storageQuery.StatusMessage}");
                                }
                            }
                            else
                            {
                                _logger.LogCritical($"channel[storage-marketplace.addMarketplaceStorage] addr create fail: {addrQuery.StatusMessage}");
                            }
                        }
                        else
                        {
                            _logger.LogCritical($"msg delivery error, channel[storage-marketplace.addMarketplaceStorage], null-exId");
                        }
                    });

                consumer.Start(token.Token);
            }
            catch (Exception exp)
            {
                _logger.LogError("spec erorr: " + exp.Message);
            }
        }
    }
}
