using Common.Repositories;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Storage.Data.Context;
using Storage.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace Application.IngegrationTests.StorageService
{
    public class StorageServiceIntegrationTestApiFactory : WebApplicationFactory<StorageApiProgramEntrypoint>, IAsyncLifetime
    {
        private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server")
            .WithPassword("SamplePassword!")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<StorageDbContext>>();

                string con = _mssqlContainer.GetConnectionString();

                services.AddDbContext<StorageDbContext>(options =>
                {
                    options.UseSqlServer(con);
                }, optionsLifetime: ServiceLifetime.Singleton);
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public async Task InitializeAsync()
        {
            await _mssqlContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _mssqlContainer.StopAsync();
        }
    }
}
