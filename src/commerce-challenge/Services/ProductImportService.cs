using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace commerce_challenge.Services
{
    public class ProductImportService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
