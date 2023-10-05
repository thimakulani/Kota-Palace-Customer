using Plugin.CloudFirestore.Attributes;
using System.Collections.Generic;

namespace Kota_Palace.Models
{
    public class MenuModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Url { get; set; }
        public bool Status { get; set; }
        public int BusinessId { get; set; }
        public ICollection<Extras> Extras { get; set; } //

    }
}
public class Extras
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int MenuId { get; set; }
}