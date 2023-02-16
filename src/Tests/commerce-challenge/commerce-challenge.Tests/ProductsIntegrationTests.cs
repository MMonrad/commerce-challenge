using System;
using System.Net.Http;
using System.Threading.Tasks;
using commerce_challenge.Models.ViewModels;
using Newtonsoft.Json;
using Xunit;

namespace commerce_challenge.Tests
{
    public class ProductsIntegrationTests : IClassFixture<CommerceServerFixture>
    {
        private readonly CommerceServerFixture _fixture;

        public ProductsIntegrationTests(CommerceServerFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task GET_ALL_PRODUCTS_OK()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/products");
            var response = await _fixture.Client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var viewModel = JsonConvert.DeserializeObject<ProductsViewModel>(responseContent);
            Assert.Equal(100, viewModel?.Products.Count);
        }
    }
}
