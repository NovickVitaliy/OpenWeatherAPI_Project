namespace ServiceContracts;

public interface ICityService
{
    Task<List<Dictionary<string, object>?>> GetCityDetails(string cityName);
}