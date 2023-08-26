namespace Entities;

public class WeatherInfo
{
    public string? Latitude { get; set; }
    public string? Longtitude { get; set; }
    public string? Timezone { get; set; }
    public CurrentWeather? CurrentWeather { get; set; }
}