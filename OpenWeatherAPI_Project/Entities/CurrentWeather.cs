namespace Entities;

public class CurrentWeather
{
    public DateTime? TimeOfTheDay { get; set; }
    public double Temperature { get; set; }
    public double Pressure { get; set; }
    public double Humidity { get; set; }
    public double CloudsPercentage { get; set; }
    public double VisibilityInMetres { get; set; }
    public double WindSpeed { get; set; }
    public string? MainWeather { get; set; }
    public string? WeatherDescription { get; set; }
    public string? WeatherIcon { get; set; }
}