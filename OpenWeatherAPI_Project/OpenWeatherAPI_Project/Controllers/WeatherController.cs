using System.Globalization;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using ServiceContracts;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace OpenWeatherAPI_Project.Controllers;

[Route("[controller]")]
public class WeatherController : Controller
{
    private readonly ICityService _cityService;
    private readonly IWeatherService _weatherService;

    public WeatherController(ICityService cityService, IWeatherService weatherService)
    {
        _cityService = cityService;
        _weatherService = weatherService;
    }
    
    [Route("/")]
    [Route("[action]")]
    public async Task<IActionResult> Index(string? cityName)
    {
        List<CityDetails>? cities = null;
        
        if (cityName != null)
        {
            ViewBag.CurrentCity = cityName;
            cities = new List<CityDetails>();
            var cityInfo = await _cityService.GetCityDetails(cityName);

            foreach (var city in cityInfo)
            {
                var cityObj = new CityDetails()
                {
                    CityName = city["name"].ToString(),
                    CountryName = city["country"].ToString(),
                    Latitude = Convert.ToDouble(city["lat"].ToString(), CultureInfo.InvariantCulture),
                    Longtitude = Convert.ToDouble(city["lon"].ToString(), CultureInfo.InvariantCulture)
                };
                if (city.ContainsKey("state"))
                {
                    cityObj.State = city["state"].ToString();
                }
                cities.Add(cityObj);
            }
        }
        
        return View(cities);
    }

    [Route("[action]")]
    public async Task<IActionResult> WeatherInfo(string? latitudeStr, string? longtitudeStr, string cityName, string countryName, string state)
    {
        if (latitudeStr == null && longtitudeStr == null)
            throw new ArgumentException();

        double latitude = Convert.ToDouble(latitudeStr);
        double longtitude = Convert.ToDouble(longtitudeStr);

        var data = await _weatherService.GetWeatherDetails(longtitude, latitude);
        var weather = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(data["weather"].ToString())[0];
        
        Entities.WeatherInfo weatherInfo = new WeatherInfo()
        {
            Longtitude = DeserializeJsonNestedData(data["coord"])["lon"].ToString(),
            Latitude = DeserializeJsonNestedData(data["coord"])["lat"].ToString(),
            Timezone = data["timezone"].ToString(),
            CurrentWeather = new CurrentWeather()
            {
                CloudsPercentage = Convert.ToDouble(DeserializeJsonNestedData(data["clouds"])["all"].ToString(), CultureInfo.InvariantCulture),
                Humidity = Convert.ToDouble(DeserializeJsonNestedData(data["main"])["humidity"].ToString() ,CultureInfo.InvariantCulture),
                MainWeather = weather["main"].ToString(),
                Pressure = Convert.ToDouble(DeserializeJsonNestedData(data["main"])["pressure"].ToString() , CultureInfo.InvariantCulture),
                Temperature = Convert.ToDouble(DeserializeJsonNestedData(data["main"])["temp"].ToString() , CultureInfo.InvariantCulture),
                TimeOfTheDay = new DateTime(1970, 1, 1, 0,0,0,0).AddSeconds(Convert.ToDouble(data["dt"].ToString(), CultureInfo.InvariantCulture)).ToLocalTime(),
                VisibilityInMetres = Convert.ToDouble(data["visibility"].ToString(), CultureInfo.InvariantCulture) ,
                WeatherDescription = weather["description"].ToString(),
                WeatherIcon = weather["icon"].ToString(),
                WindSpeed = Convert.ToDouble(DeserializeJsonNestedData(data["wind"])["speed"].ToString(), CultureInfo.InvariantCulture)
            }
        };

        if (weatherInfo == null)
            return StatusCode(500);

        ViewBag.City = cityName;
        ViewBag.Country = countryName;
        ViewBag.State = state;
        
        return View(weatherInfo);
    }

    private Dictionary<string, object> DeserializeJsonNestedData(object json)
    {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json.ToString());
    }
}