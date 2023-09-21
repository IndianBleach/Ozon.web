// See https://aka.ms/new-console-template for more information
using Tests;
using Tests.Streams;

Console.WriteLine("Hello, World!");

Utils utils = new Utils();

StorageStream storageStream = new StorageStream();

Task.Run(async () =>
{
    var t1 = storageStream.Stream_MarketProductsStorageRegistrations(1);
    var t2 = storageStream.Stream_MarketProductsStorageRegistrations(1);
    var t3 = storageStream.Stream_MarketProductsStorageRegistrations(1);

    Task.WaitAll(t1, t2, t3);
}).Wait();