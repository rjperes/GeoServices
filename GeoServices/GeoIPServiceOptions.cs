namespace GeoServices
{
    public class GeoIPServiceOptions
    {
        public string? ApiKey { get; set; }
        public TimeSpan? CacheDuration { get; set; }
    }
}
