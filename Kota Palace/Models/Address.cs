
namespace Kota_Palace.Models
{
    public class Address
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int BusinessId { get; set; }
        public SpazaModel Business { get; set; }
    }
}
