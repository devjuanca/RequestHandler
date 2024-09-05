namespace SampleAPI.Dtos;

public class CreateCityForecastCommand
{
    public required string City { get; set; }

    public required WeatherForecast WeatherForecast { get; set; }
}
