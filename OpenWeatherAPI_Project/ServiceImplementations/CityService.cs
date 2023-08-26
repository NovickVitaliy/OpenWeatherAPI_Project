using System.Globalization;
using ServiceContracts;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ServiceImplementations;

public class CityService : ICityService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public CityService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }
    
    public async Task<List<Dictionary<string, object>?>> GetCityDetails(string cityName)
    {
        string? apiToken = _configuration.GetValue<string>("openweatherapitoken");

        if (apiToken is null)
            throw new ArgumentNullException(apiToken);
        
        using (var httpClient = _clientFactory.CreateClient())
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri($"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&limit=5&appid={apiToken}"),
                Method = HttpMethod.Get
            };

            var responseMessage = await httpClient.SendAsync(requestMessage);

            var json = await (new StreamReader(await responseMessage.Content.ReadAsStreamAsync())).ReadToEndAsync();

            List<Dictionary<string, object>?> info = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json);

            return info;
        }
    }
}