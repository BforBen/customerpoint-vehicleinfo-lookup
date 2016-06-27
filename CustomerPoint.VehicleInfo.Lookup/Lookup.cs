using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CustomerPoint.VehicleInfo.Models;
using CustomerPoint.VehicleInfo.RegCheckApi;

namespace CustomerPoint.VehicleInfo
{
    public static class Lookup
    {
        public static async Task<VehicleData> Vehicle(string RegistrationNumber)
        {
            if (string.IsNullOrWhiteSpace(RegistrationNumber))
            {
                return null;
            }

            RegistrationNumber = RegistrationNumber.ToUpper();

            RegistrationNumber = Regex.Replace(RegistrationNumber, @"[^A-Z\d]+", "", RegexOptions.None);

            VehicleData vd = null;

            var Cache = MemoryCache.Default;
            var CacheName = "VehicleInfo-" + RegistrationNumber;

            // Check if data is cached already
            if (!Cache.Contains(CacheName))
            {
                var ApiClient = new CarRegSoapClient();
                //ApiClient.Endpoint.Address = new System.ServiceModel.EndpointAddress("https://www.regcheck.org.uk/api/reg.asmx");

                //var Binding = new System.ServiceModel.BasicHttpBinding();
                //Binding.Security = new System.ServiceModel.BasicHttpSecurity() { Mode = System.ServiceModel.BasicHttpSecurityMode.Transport };
                //ApiClient.Endpoint.Binding = Binding;

                var Spec = await ApiClient.CheckAsync(RegistrationNumber, Properties.Settings.Default.RegCheckApiKey);

                if (Spec != null)
                {
                    vd = new VehicleData
                    {
                        Vrm = RegistrationNumber,
                        Make = Spec.vehicleData.MakeDescription.Value,
                        Model = Spec.vehicleData.ModelDescription.Text.FirstOrDefault(),
                        Fuel = Spec.vehicleData.FuelType.CurrentTextValue.Text.FirstOrDefault()
                    };

                    // Cache result for 1 day
                    var Cached = Cache.Add(
                            new CacheItem(CacheName, vd),
                            new CacheItemPolicy()
                                {
                                    AbsoluteExpiration = System.DateTimeOffset.UtcNow.AddDays(1)
                                });
                }

                return null;
            }
            else
            {
                return (VehicleData)Cache.Get(CacheName);
            }
        }
    }
}
