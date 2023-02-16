using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Commerce;

namespace commerce_challenge.Core.Http
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public AuthenticationHandler(HttpMessageHandler innerHandler, IHttpClientFactory httpClientFactory)
            : base(innerHandler)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using var http = _httpClientFactory.CreateClient();

            var client = new CommerceClient(http);
            var login = await client.LoginAsync(new LoginRequest { Email = "hylkegaard@gmail.com" }, cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", login.Token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
