using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Commerce;
using commerce_challenge.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace commerce_challenge.Services
{
    public class ProductImportService : BackgroundService
    {
        private readonly IDbContextFactory<CommerceDbContext> _dbContextFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductImportService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromSeconds(5);

        public ProductImportService(ILogger<ProductImportService> logger,
            IHttpClientFactory httpClientFactory,
            IDbContextFactory<CommerceDbContext> dbContextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        internal async Task Import(CancellationToken token)
        {
            using var http = _httpClientFactory.CreateClient("commerce");
            var client = new CommerceClient(http);
            var context = await _dbContextFactory.CreateDbContextAsync(token);

            try
            {
                var rawProducts = await client.GetAllProductsAsync(token);
                var existingProducts = await context.Products.ToListAsync(token);

                var newProducts = rawProducts.Where(rawProduct => existingProducts.All(existing => existing.ExternalId != rawProduct.Id))
                    .Select(rawProduct => new ProductEntity
                    {
                        Name = rawProduct.Name, Price = new decimal(rawProduct.Price), Size = rawProduct.Size, Stars = rawProduct.Stars,
                        ExternalId = rawProduct.Id
                    })
                    .ToList();

                var updatedRawProducts = rawProducts.Where(rawProduct => existingProducts.Any(existing => existing.ExternalId == rawProduct.Id))
                    .ToList();
                foreach (var rawProduct in updatedRawProducts)
                {
                    var p = existingProducts.First(x => x.ExternalId == rawProduct.Id);
                    p.Name = rawProduct.Name;
                    p.Price = new decimal(rawProduct.Price);
                    p.Stars = rawProduct.Stars;
                    p.Size = rawProduct.Size;
                }

                var deletedProducts = existingProducts.Where(existing => rawProducts.All(rawProduct => existing.ExternalId != rawProduct.Id))
                    .ToList();

                await context.AddRangeAsync(newProducts, token);
                context.RemoveRange(deletedProducts);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Failed to execute PeriodicHostedService with exception message {ExMessage}. Good luck next round!",
                    ex.Message);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await Import(stoppingToken);
            }
        }
    }
}
