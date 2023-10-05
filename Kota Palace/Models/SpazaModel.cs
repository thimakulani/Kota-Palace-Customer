using System.ComponentModel;

namespace Kota_Palace.Models
{
    public class SpazaModel
    {
        public int Id { get; set; }
        [DisplayName("TRADING NAME")]
        public string Name { get; set; }
        [DisplayName("DESCRIPTION")]
        public string Description { get; set; }
        [DisplayName("CONTACT NUMBER")]
        public string PhoneNumber { get; set; }
        public string ImgUrl { get; set; }
        public string OwnerId { get; set; }
        [DisplayName("APPLICATION STATUS")]
        public string Status { get; set; }
        public string Online { get; set; }
        public User Owner { get; set; }
        public Address Address { get; set; }
    }
}