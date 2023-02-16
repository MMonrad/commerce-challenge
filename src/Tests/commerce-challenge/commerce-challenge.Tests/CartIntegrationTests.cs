using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using commerce_challenge.Models.Requests;
using commerce_challenge.Models.ViewModels;
using Newtonsoft.Json;
using Xunit;

namespace commerce_challenge.Tests
{
    public class CartIntegrationTests : IClassFixture<CommerceServerFixture>
    {
        private readonly CommerceServerFixture _fixture;

        public CartIntegrationTests(CommerceServerFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task AddProduct_TO_CART_OK()
        {
            var requestCart = new HttpRequestMessage(HttpMethod.Post, "/api/v1/carts");
            requestCart.Content = JsonContent.Create(new CreateCartRequest { Email = "hylkegaard@gmail.com" });
            var responseCart = await _fixture.Client.SendAsync(requestCart);
            var responseCartContent = await responseCart.Content.ReadAsStringAsync();
            var viewModel = JsonConvert.DeserializeObject<CreateCartViewModel>(responseCartContent);
            Assert.NotEmpty(viewModel?.CartId!);

            var productId = await GetProductId();
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/carts/{viewModel.CartId}");
            request.Content = JsonContent.Create(new AddToCartRequest { Quantity = 2, ProductId = productId});
            var response = await _fixture.Client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task CREATE_CART_WITH_EMAIL_OK()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/carts");
            request.Content = JsonContent.Create(new CreateCartRequest { Email = "hylkegaard@gmail.com" });
            var response = await _fixture.Client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var viewModel = JsonConvert.DeserializeObject<CreateCartViewModel>(responseContent);
            Assert.NotEmpty(viewModel?.CartId!);
        }

        [Fact]
        public async Task CREATE_CART_WITHOUT_EMAIL_BADREQUEST()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/carts");
            request.Content = JsonContent.Create("");
            var response = await _fixture.Client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private async Task<string> GetProductId()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/products");
            var response = await _fixture.Client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var viewModel = JsonConvert.DeserializeObject<ProductsViewModel>(responseContent);
            return viewModel?.Products.First()
                .Id!;
        }
    }
}
