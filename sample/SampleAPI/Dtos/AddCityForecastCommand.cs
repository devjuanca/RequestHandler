namespace SampleAPI.Dtos;

public class AddCityForecastCommand
{
    public required string City { get; set; }

    public required WeatherForecast WeatherForecast { get; set; }
}
