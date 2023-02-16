using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using commerce_challenge.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace commerce_challenge.Controllers.v1
{
    [Route("api/v1/products")]
    public class ProductsController : CommerceControllerBase
    {
        private readonly IDbContextFactory<CommerceDbContext> _dbContextFactory;

        public ProductsController(IDbContextFactory<CommerceDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        /// <summary>
        /// 
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductsViewModel))]
        [HttpGet]
        public async Task<ActionResult<ProductsViewModel>> GetProducts(CancellationToken token)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(token);
            var entities = await context.Products.OrderBy(x => x.Stars)
                .Take(100)
                .ToListAsync(token);

            var products = entities.Select(entity => new ProductViewModel())
                .ToList();

            return new ProductsViewModel { Products = products };
        }
    }
}
