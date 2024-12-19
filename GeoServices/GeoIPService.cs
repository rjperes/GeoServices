using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace GeoServices
{
    public class IP2LocationGeoIPService : IGeoIPService
    {
        class IP2LocationGeoInfo : GeoInfo
        {
            public string? IP { get; set; }
            public string? RegionName { get; set; }
            public string? CityName { get; set; }
            public string? ZipCode { get; set; }
            public string? TimeZone
            {
                get
                {
                    return base.Timezone;
                }
                set
                {
                    base.Timezone = value;
                }
            }
            public string? Asn { get; set; }
            public string? As { get; set; }
            public bool IsProxy { get; set; }

            public override string ToString()
            {
                var str = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(CityName))
                {
                    str.Append(CityName);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(RegionName))
                {
                    str.Append(RegionName);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(CountryName))
                {
                    str.Append(CountryName);
                }

                return str.ToString().TrimEnd(',', ' ');
            }
        }

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public IP2LocationGeoIPService(HttpClient httpClient, IOptions<GeoIPServiceOptions> options)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
            ArgumentException.ThrowIfNullOrEmpty(options?.Value.ApiKey, nameof(GeoIPServiceOptions.ApiKey));

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ip2location.io/");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");

            _apiKey = options.Value.ApiKey;
        }

        public async Task<GeoInfo> GetInfo(string ipAddress, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));

            var geoInfo = await _httpClient.GetFromJsonAsync<IP2LocationGeoInfo>($"?ip={ipAddress}&key={_apiKey}", _options, cancellationToken);
            return geoInfo!;
        }
    }

    public class IPGeolocationGeoIPService : IGeoIPService
    {
        class Currency
        {
            public string? Name { get; set; }
            public string? Code { get; set; }
            public string? Symbol { get; set; }
        }

        class Timezone
        {
            public string? Name { get; set; }
            public int Offset { get; set; }
            public int OffsetWithDst { get; set; }
            public DateTime CurrentTime { get; set; }
            public double CurrentTimeUnix { get; set; }
            public bool IsDst { get; set; }
            public int DstSavings { get; set; }
            public bool DstExists { get; set; }
            public Dst? DstStart { get; set; }
            public Dst? DstEnd { get; set; }
        }

        class Dst
        {
            public string? UtcTime { get; set; }
            public string? Duration { get; set; }
            public bool Gap { get; set; }
            public string? DateTimeAfter { get; set; }
            public string? DateTimeBefore { get; set; }
            public bool Overlap { get; set; }
        }

        class IPGeolocationGeoInfo : GeoInfo
        {
            public string? IP { get; set; }
            public string? Hostname { get; set; }
            public string? ContinentCode { get; set; }
            public string? ContinentName { get; set; }
            public string? CountryCode2
            {
                get
                {
                    return CountryCode;
                }
                set
                {
                    CountryCode = value;
                }
            }
            public string? CountryCode3 { get; set; }
            public string? CountryCapital { get; set; }
            public string? StateProv { get; set; }
            public string? District { get; set; }
            public string? City { get; set; }
            public string? ZipCode { get; set; }
            public new string? Latitude
            {
                get
                {
                    return base.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        base.Latitude = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
            public new string? Longitude
            {
                get
                {
                    return base.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
                set
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        base.Longitude = double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
            }
            public bool IsEu { get; set; }
            public string? CallingCode { get; set; }
            public string? CountryTld { get; set; }
            public string? Languages { get; set; }
            public string? CountryFlag { get; set; }
            public string? Isp { get; set; }
            public string? ConnectionType { get; set; }
            public string? Organization { get; set; }
            public string? CountryEmoji { get; set; }
            public string? GeoNameId { get; set; }
            public Currency? Currency { get; set; }
            public new Timezone? Timezone { get; set; }

            public override string ToString()
            {
                var str = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(City))
                {
                    str.Append(City);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(District))
                {
                    str.Append(District);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(CountryName))
                {
                    str.Append(CountryName);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(ContinentName))
                {
                    str.Append(ContinentName);
                }

                return str.ToString().TrimEnd(',', ' ');
            }
        }

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public IPGeolocationGeoIPService(HttpClient httpClient, IOptions<GeoIPServiceOptions> options)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
            ArgumentException.ThrowIfNullOrEmpty(options?.Value.ApiKey, nameof(GeoIPServiceOptions.ApiKey));

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.ipgeolocation.io/ipgeo");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");

            _apiKey = options.Value.ApiKey;
        }

        public async Task<GeoInfo> GetInfo(string ipAddress, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));

            var geoInfo = await _httpClient.GetFromJsonAsync<IPGeolocationGeoInfo>($"?ip={ipAddress}&apiKey={_apiKey}", _options, cancellationToken);
            return geoInfo!;
        }
    }

    public class IPLocationGeoIPService : IGeoIPService
    {
        class IPLocationGeoInfo : GeoInfo
        {
            public string? IP { get; set; }
            public string? IPNumber { get; set; }
            public int IPVersion { get; set; }
            public string? CountryCode2
            {
                get
                {
                    return CountryCode;
                }
                set
                {
                    CountryCode = value;
                }
            }
            public string? Isp { get; set; }
        }

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        private readonly HttpClient _httpClient;

        public IPLocationGeoIPService(HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.iplocation.net/");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");
        }

        public async Task<GeoInfo> GetInfo(string ipAddress, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));

            var geoInfo = await _httpClient.GetFromJsonAsync<IPLocationGeoInfo>($"?ip={ipAddress}", _options, cancellationToken);

            return geoInfo!;
        }
    }

    public class HackerTargetGeoIPService : IGeoIPService
    {
        class HackerTargetGeoInfo : GeoInfo
        {
            public string? IpAddress { get; set; }
            public string? State { get; set; }
            public string? City { get; set; }
        }

        private readonly HttpClient _httpClient;

        public HackerTargetGeoIPService(HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.hackertarget.com/ipgeo/");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");
        }

        public async Task<GeoInfo> GetInfo(string ipAddress, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));

            var res = await _httpClient.GetStringAsync($"?q={ipAddress}", cancellationToken);
            var geo = new HackerTargetGeoInfo();

            if (!string.IsNullOrWhiteSpace(res))
            {
                var lines = res.Split('\n');

                geo.IpAddress = lines[0].Split(':')[1]?.Trim();
                geo.CountryName = lines[1].Split(':')[1]?.Trim();
                geo.State = lines[2].Split(':')[1]?.Trim();
                geo.City = lines[3].Split(':')[1]?.Trim();
                geo.Latitude = float.Parse(lines[4].Split(':')[1]);
                geo.Longitude = float.Parse(lines[5].Split(':')[1]);
            }

            return geo;
        }
    }

    public class IP_ApiGeoService : IGeoIPService
    {
        class IP_ApiGeoInfo : GeoInfo
        {
            public string? Query { get; set; }
            public string? Status { get; set; }
            public string? Country
            {
                get
                {
                    return CountryName;
                }
                set
                {
                    CountryName = value;
                }
            }
            public string? Region { get; set; }
            public string? RegionName { get; set; }
            public string? City { get; set; }
            public string? Zip { get; set; }
            public double Lat
            {
                get
                {
                    return Latitude;
                }
                set
                {
                    Latitude = value;
                }
            }
            public double Lon
            {
                get
                {
                    return Longitude;
                }
                set
                {
                    Longitude = value;
                }
            }
            public string? Isp { get; set; }
            public string? Org { get; set; }
            public string? As { get; set; }

            public override string ToString()
            {
                var str = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(City))
                {
                    str.Append(City);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(RegionName))
                {
                    str.Append(RegionName);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(CountryName))
                {
                    str.Append(CountryName);
                    str.Append(", ");
                }

                return str.ToString().TrimEnd(',', ' ');
            }
        }

        private readonly HttpClient _httpClient;

        public IP_ApiGeoService(HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://ip-api.com/json/");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");
        }

        public async Task<GeoInfo> GetInfo(string ipAddress, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));

            var geoInfo = await _httpClient.GetFromJsonAsync<IP_ApiGeoInfo>(ipAddress, cancellationToken);

            return geoInfo!;
        }
    }

    public class IPApiGeoIPService : IGeoIPService
    {
        class IPApiGeoInfo : GeoInfo
        {
            public string? Ip { get; set; }
            public string? Network { get; set; }
            public string? Version { get; set; }
            public string? City { get; set; }
            public string? Region { get; set; }
            public string? RegionCode { get; set; }
            public string? Country
            {
                get
                {
                    return CountryName;
                }
                set
                {
                    CountryName = value;
                }
            }
            public string? CountryCodeIso3 { get; set; }
            public string? CountryCapital { get; set; }
            public string? CountryTld { get; set; }
            public string? ContinentCode { get; set; }
            public bool InEu { get; set; }
            public string? Postal { get; set; }
            public string? UtcOffset { get; set; }
            public string? CountryCallingCode { get; set; }
            public string? Currency { get; set; }
            public string? CurrencyName { get; set; }
            public string? Languages { get; set; }
            public string? CountryArea { get; set; }
            public long CountryPopulation { get; set; }
            public string? Asn { get; set; }
            public string? Org { get; set; }

            public override string ToString()
            {
                var str = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(City))
                {
                    str.Append(City);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(Region))
                {
                    str.Append(Region);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(CountryName))
                {
                    str.Append(CountryName);
                }

                return str.ToString().TrimEnd(',', ' ');
            }
        }

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        private readonly HttpClient _httpClient;

        public IPApiGeoIPService(HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://ipapi.co/");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");
        }

        public async Task<GeoInfo> GetInfo(string ipAddress, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));

            var geoInfo = await _httpClient.GetFromJsonAsync<IPApiGeoInfo>($"{ipAddress}/json", _options, cancellationToken);

            return geoInfo!;
        }
    }

    public class FreeIPApiGeoIPService : IGeoIPService
    {
        class CurrencyInfo
        {
            public required string Code { get; set; }
            public required string Name { get; set; }

            public override string ToString() => Name;
        }

        class FreeIPApiGeoInfo : GeoInfo
        {
            public int IpVersion { get; set; }
            public string? IpAddress { get; set; }
            public string? ZipCode { get; set; }
            public string? CityName { get; set; }
            public string? RegionName { get; set; }
            public bool IsProxy { get; set; }
            public string? Continent { get; set; }
            public string? ContinentCode { get; set; }
            public CurrencyInfo? Currency { get; set; }
            public string? Language { get; set; }
            public string[]? Timezones { get; set; }
            public string[]? Tlds { get; set; }

            public override string ToString()
            {
                var str = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(CityName))
                {
                    str.Append(CityName);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(RegionName))
                {
                    str.Append(RegionName);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(CountryName))
                {
                    str.Append(CountryName);
                    str.Append(", ");
                }

                if (!string.IsNullOrWhiteSpace(Continent))
                {
                    str.Append(Continent);
                }

                return str.ToString().TrimEnd(',', ' ');
            }
        }

        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly HttpClient _httpClient;

        public FreeIPApiGeoIPService(HttpClient httpClient)
        {
            ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://freeipapi.com/api/json/");
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, MediaTypeNames.Application.Json);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.CacheControl, "public, max-age=3600");
        }

        public async Task<GeoInfo> GetInfo(string ipAddress, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrEmpty(ipAddress, nameof(ipAddress));

            var geoInfo = await _httpClient.GetFromJsonAsync<FreeIPApiGeoInfo>(ipAddress, _options, cancellationToken);

            return geoInfo!;
        }
    }
}