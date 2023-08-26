using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ServiceContracts;

namespace ServiceImplementations;

public class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public WeatherService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    public async Task<Dictionary<string, object>?> GetWeatherDetails(double? lon, double? lat)
    {
        string? token = _configuration.GetValue<string>("openweatherapitoken");

        if (token is null)
            throw new ArgumentNullException(token);

        if (lon == null || lat == null)
            throw new ArgumentException("longtitude ot latitude equals zero");

        using (var client = _clientFactory.CreateClient())
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={token}"),
                Method = HttpMethod.Get
            };

            var response = await client.SendAsync(requestMessage);

            var json = await (new StreamReader(await response.Content.ReadAsStreamAsync())).ReadToEndAsync();

            var data = JsonSerializer.Deserialize<Dictionary<string,object>?>(json);

            return data;
        }
    }
}