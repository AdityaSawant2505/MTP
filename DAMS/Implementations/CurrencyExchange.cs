using DAMS.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DAMS.Implementations
{
    public class CurrencyExchange : ICurrencyExchange
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public CurrencyExchange(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _apiKey = _configuration["ThirdParty:apikey"];
            _httpClient = httpClient;
        }

        public async Task<string> GetCurrencyExchangeAsync(string curr)
        {
            string url = $"https://v6.exchangerate-api.com/v6/{_apiKey}/latest/{curr}";
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
    }
}
