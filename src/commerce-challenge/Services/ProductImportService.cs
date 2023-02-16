using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Commerce;
using commerce_challenge.Models;
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var http = _httpClientFactory.CreateClient("commerce");
                var client = new CommerceClient(http);
                var context = await _dbContextFactory.CreateDbContextAsync(stoppingToken);

                try
                {
                    var rawProducts = await client.GetAllProductsAsync(stoppingToken);
                    var existingProducts = await context.Products.ToListAsync(stoppingToken);

                    var newProducts = rawProducts.Where(rawProduct => existingProducts.All(existing => existing.ExternalId != rawProduct.Id))
                        .Select(rawProduct => new ProductEntity
                        {
                            Name = rawProduct.Name, Price = new decimal(rawProduct.Price), Size = rawProduct.Size, Stars = rawProduct.Stars,
                            ExternalId = rawProduct.Id
                        })
                        .ToList();
                    var updatedProducts = rawProducts.Where(rawProduct => existingProducts.Any(existing => existing.ExternalId == rawProduct.Id))
                        .Select(rawProduct => new ProductEntity
                        {
                            Name = rawProduct.Name, Price = new decimal(rawProduct.Price), Size = rawProduct.Size, Stars = rawProduct.Stars,
                            ExternalId = rawProduct.Id
                        })
                        .ToList();
                    var deletedProducts = existingProducts.Where(existing => rawProducts.All(rawProduct => existing.ExternalId != rawProduct.Id))
                        .ToList();

                    await context.AddRangeAsync(newProducts, stoppingToken);
                    context.UpdateRange(updatedProducts);
                    context.RemoveRange(deletedProducts);
                    await context.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Failed to execute PeriodicHostedService with exception message {ExMessage}. Good luck next round!",
                        ex.Message);
                }
            }
        }
    }
}
