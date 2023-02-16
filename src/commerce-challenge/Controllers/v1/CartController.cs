using System;
using System.Threading;
using System.Threading.Tasks;
using commerce_challenge.Models.Entities;
using commerce_challenge.Models.Requests;
using commerce_challenge.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace commerce_challenge.Controllers.v1
{
    [Route("api/v1/carts")]
    public class CartController : CommerceControllerBase
    {
        private readonly IDbContextFactory<CommerceDbContext> _dbContextFactory;

        public CartController(IDbContextFactory<CommerceDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{id}")]
        public async Task<ActionResult> AddToCart([FromRoute] string id, [FromBody] AddToCartRequest request, CancellationToken token)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(token);
            var product = await context.Products.FirstOrDefaultAsync(x => x.Id == new Guid(request.ProductId), token);
            if (product is null)
            {
                return BadRequest("No product found with matching id");
            }

            var cart = await context.Carts.FirstOrDefaultAsync(x => x.Id == new Guid(id), token);
            if (cart is null)
            {
                return NotFound();
            }

            await context.AddAsync(new OrderLineEntity
                    { ProductId = product.Id, Cart = cart, ProductName = product.Name, ProductUnitPrice = product.Price, Quantity = request.Quantity },
                token);
            await context.SaveChangesAsync(token);

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateCartViewModel))]
        [HttpPost]
        public async Task<ActionResult<CreateCartViewModel>> CreateCart(CreateCartRequest request, CancellationToken token)
        {
            await using var context = await _dbContextFactory.CreateDbContextAsync(token);
            var cart = new CartEntity { Email = request.Email };
            await context.Carts.AddAsync(cart, token);
            await context.SaveChangesAsync(token);

            return new CreateCartViewModel { CartId = cart.Id.ToString() };
        }
    }
}
