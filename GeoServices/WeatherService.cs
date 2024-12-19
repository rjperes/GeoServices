using GeoServices;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;

public class OpenWeatherMapWeatherService : IWeatherService
{
    class OpenWeatherMapWeatherInfo : WeatherInfo
    {
        public Coord? Coord { get; set; }
        public Weather[]? Weather { get; set; }
        public string? Base { get; set; }
        public Main? Main { get; set; }
        public int Visibility { get; set; }
        public Wind? Wind { get; set; }
        public Rain? Rain { get; set; }
        public Clouds? Icon { get; set; }
        public int Dt { get; set; }
        public Sys? Sys { get; set; }
        public int Timezone { get; set; }
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Cod { get; set; }

        public override double Longitude => ((double?)Coord?.Lon).GetValueOrDefault();
        public override double Latitude => ((double?)Coord?.Lat).GetValueOrDefault();
        public override float Temperature => ((float?)Main?.Temp).GetValueOrDefault();
    }

    class Coord
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
    }

    public record Main(float Temp, float FeelsLike, float TempMin, float TempMax, int Pressure, int Humidity, int SeaLevel, int GrndLevel);

    public record Wind(float Speed, int Deg, float Gust);

    public record Rain(float Lh);

    public record Clouds(int All);

    public record Sys(int Type, int Id, string? Country, int Sunrise, int Sunset);

    public record Weather(int Id, string? Main, string? Description, string? Icon);

    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    public OpenWeatherMapWeatherService(HttpClient httpClient, IOptions<WeatherServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentException.ThrowIfNullOrWhiteSpace(options.Value.ApiKey, nameof(WeatherServiceOptions.ApiKey));

        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(@"https://api.openweathermap.org/data/2.5/weather");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");

        _apiKey = options.Value.ApiKey;
    }

    public async Task<WeatherInfo> GetWeather(double lat, double lon, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<OpenWeatherMapWeatherInfo>($"?lat={lat}&lon={lon}&appid={_apiKey}", _options, cancellationToken);
    }
}

public class OpenMeteoWeatherService : IWeatherService
{
    class OpenMeteoWeatherInfo : WeatherInfo
    {
        public float GenerationtimeMs { get; set; }
        public int UtcOffsetSeconds { get; set; }
        public string? Timezone { get; set; }
        public string? TimezoneAbbreviation { get; set; }
        public float Elevation { get; set; }
        public CurrentUnits? CurrentUnits { get; set; }
        public Current? Current { get; set; }
        //public HourlyUnits? HourlyUnits { get; set; }
        //public Hourly? Hourly { get; set; }
        //public DailyUnits? DailyUnits { get; set; }
        //public Daily? Daily { get; set; }

        public override float Temperature => ((float?)Current?.Temperature2m).GetValueOrDefault();
    }

    record CurrentUnits
    (
        string? Time,
        string? Interval,
        [property: JsonPropertyName("temperature_2m")] string? Temperature2m,
        [property: JsonPropertyName("wind_speed_10m")] string? WindSpeed10m,
        [property: JsonPropertyName("relative_humidity_2m")] string? RelativeHumidity2m,
        string? ApparentTemperature,
        string? IsDay,
        string? Precipitation,
        string? Rain,
        string? Showers,
        string? Snowfall,
        string? WeatherCode,
        string? CloudCover,
        string? PressureMsl,
        string? SurfacePressure,
        [property: JsonPropertyName("wind_direction_10m")] string? WindDirection10m,
        [property: JsonPropertyName("wind_gusts_10m")] string? WindGusts10m
    );

    record Current
    (
        string? Time,
        int Interval,
        [property: JsonPropertyName("temperature_2m")] float Temperature2m,
        [property: JsonPropertyName("wind_speed_10m")] float WindSpeed10m,
        [property: JsonPropertyName("relative_humidity_2m")] int RelativeHumidity2m,
        float ApparentTemperature,
        int IsDay,
        float Precipitation,
        float Rain,
        float Showers,
        float Snowfall,
        //https://www.nodc.noaa.gov/archive/arc0021/0002199/1.1/data/0-data/HTML/WMO-CODE/WMO4677.HTM
        int WeatherCode,
        int CloudCover,
        float PressureMsl,
        float SurfacePressure,
        [property: JsonPropertyName("wind_direction_10m")] int WindDirection10m,
        [property: JsonPropertyName("wind_gusts_10m")] float WindGusts10m
    );

    record DailyUnits
    (
        string? Time,
        string? WeatherCode,
        [property: JsonPropertyName("temperature_2m_max")] string? Temperature2mMax,
        [property: JsonPropertyName("temperature_2m_min")] string? Temperature2mMin
    );

    record HourlyUnits
    (
        string? Time,
        [property: JsonPropertyName("temperature_2m")] string? Temperature2m,
        [property: JsonPropertyName("relative_humidity_2m")] string? RelativeHumidity2m,
        [property: JsonPropertyName("wind_speed_10m")] string? WindSpeed10m
    );

    record Hourly
    (
        string[]? Time,
        [property: JsonPropertyName("temperature_2m")] float[]? Temperature2m,
        [property: JsonPropertyName("relative_humidity_2m")] int[]? RelativeHumidity2m,
        [property: JsonPropertyName("wind_speed_10m")] float[]? WindSpeed10m
    );

    record Daily
    (
        string[]? Time,
        int WeatherCode,
        [property: JsonPropertyName("temperature_2m_max")] float Temperature2mMax,
        [property: JsonPropertyName("temperature_2m_min")] float Temperature2mMin
    );

    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private readonly HttpClient _httpClient;

    public OpenMeteoWeatherService(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(@"https://api.open-meteo.com/v1/forecast?");
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
        _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");
    }

    public async Task<WeatherInfo> GetWeather(double lat, double lon, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<OpenMeteoWeatherInfo>($"?latitude={lat}&longitude={lon}&current=temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,snowfall,weather_code,cloud_cover,pressure_msl,surface_pressure,wind_speed_10m,wind_direction_10m,wind_gusts_10m&daily=weather_code,temperature_2m_max,temperature_2m_min", _options, cancellationToken);
    }
}