using EpMon.Helpers;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace EpMon.Infrastructure
{
    public interface ITokenService
    {
        string GetToken(bool forceFreshToken = false);
    }

    public class CachedTokenService : ITokenService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        private readonly object _lockObject = new object();

        private string _cachedToken = string.Empty;
        private DateTime _cachedTokenExpiration = DateTime.MinValue;

        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tokenService;
        private readonly string _scope;

        public CachedTokenService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<CachedTokenService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            _clientId = config.GetSection("TokenService:ClientId").Value;
            _clientSecret = config.GetSection("TokenService:ClientSecret").Value;
            _tokenService = config.GetSection("TokenService:TokenService").Value;
            _scope = config.GetSection("TokenService:Scope").Value;
        }

        public string GetToken(bool forceFreshToken = false)
        {
            if (!IsTokenSecurityConfigured())
            {
                return string.Empty;
            }

            lock (_lockObject)
            {
                if (forceFreshToken || !IsValidToken())
                {
                    RefreshToken();
                }
                return _cachedToken;
            }
        }

        private bool IsTokenSecurityConfigured()
        {
            return !string.IsNullOrEmpty(_tokenService);
        }

        private bool IsValidToken()
        {
            return string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _cachedTokenExpiration;
        }

        private void RefreshToken()
        {
            var client = _httpClientFactory.CreateClient(UriHelper.GetBaseUri(_tokenService));

            var tokenResponse = AsyncHelper.RunSync(() => client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _tokenService,
                Scope = _scope,
                ClientId = _clientId,
                ClientSecret = _clientSecret
            }));

            if (tokenResponse == null || tokenResponse.IsError)
            {
                _cachedTokenExpiration = DateTime.MinValue;
                _cachedToken = string.Empty;

                _logger.LogError($"Error requesting token : ({tokenResponse?.ErrorDescription} - {tokenResponse?.HttpErrorReason})");
            }
            else
            {
                _cachedTokenExpiration = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 5);
                _cachedToken = tokenResponse.AccessToken;
            }
        }
    }
}
