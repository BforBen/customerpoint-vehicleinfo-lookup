using System.Linq;
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
            if (!string.IsNullOrWhiteSpace(RegistrationNumber))
            {
                return null;
            }

            RegistrationNumber = RegistrationNumber.ToUpper();

            RegistrationNumber = Regex.Replace(RegistrationNumber, @"[^A-Z\d]+", "", RegexOptions.None);

            var ApiClient = new CarRegSoapClient();
            var Spec = await ApiClient.CheckAsync(RegistrationNumber, Properties.Settings.Default.RegCheckApiKey);

            if (Spec != null)
            {
                return new VehicleData
                    {
                        Vrm = RegistrationNumber,
                        Make = Spec.vehicleData.MakeDescription.Value,
                        Model = Spec.vehicleData.ModelDescription.Text.FirstOrDefault(),
                        Fuel = Spec.vehicleData.FuelType.CurrentTextValue.Text.FirstOrDefault()
                    };
            }

            return null;
        }
    }
}
