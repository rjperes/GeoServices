namespace GeoServices
{
    public interface IWeatherService
    {
        Task<WeatherInfo> GetWeather(double lat, double lon, CancellationToken cancellationToken = default);
    }

    public class WeatherInfo
    {
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual float Temperature { get; set; }
    }
}