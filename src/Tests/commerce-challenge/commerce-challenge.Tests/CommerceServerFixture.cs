using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using commerce_challenge.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace commerce_challenge.Tests
{
    public class CommerceServerFixture : IDisposable, IAsyncLifetime
    {
        private readonly TestServer _testServer;
        public HttpClient Client { get; }

        public CommerceServerFixture()
        {
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            Client = _testServer.CreateClient();
        }

        public void Dispose()
        {
            _testServer.Dispose();
            Client.Dispose();
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            var productImportService = _testServer.Services.GetService<ProductImportService>();
            await productImportService?.Import(CancellationToken.None)!;
        }
    }
}
