namespace ServiceContracts;

public interface IWeatherService
{
    Task<Dictionary<string, object>?> GetWeatherDetails(double? lon, double? lat);
}