using System.ComponentModel.DataAnnotations;

namespace CustomerPoint.VehicleInfo.Models
{
    public class VehicleData
    {
        [Display(Name = "Registration number")]
        public string Vrm { get; internal set; }
        public string Make { get; internal set; }
        public string Model { get; internal set; }
        public string Fuel { get; internal set; }
    }
}
