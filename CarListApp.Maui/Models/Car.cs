using SQLite;

namespace CarListApp.Maui.Models
{
    [Table("cars")]
    public class Car : BaseEntity
    {
        public string Make { get; set; }

        public string Model { get; set; }

        [Unique, MaxLength(12)]
        public string Vin { get; set; }
    }
}
