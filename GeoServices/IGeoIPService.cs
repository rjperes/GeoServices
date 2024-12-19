namespace GeoServices
{
    public class GeoInfo
    {
        public string? CountryName { get; set; }
        public string? CountryCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Timezone { get; set; }

        public override string ToString() => CountryName!;
    }

    public interface IGeoIPService
    {
        Task<GeoInfo> GetInfo(string ipAddress, CancellationToken cancellationToken = default);
    }
}