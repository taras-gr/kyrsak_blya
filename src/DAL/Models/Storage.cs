using Newtonsoft.Json;
using System.Collections.Generic;

namespace DAL.Models
{
    /// <summary>
    /// Storage model for create entity in database
    /// </summary>
    public class Storage
    {
        public Storage()
        {
            Products = new List<GoodStorage>();
        }

        public int Id { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        [JsonIgnore]
        public ICollection<GoodStorage> Products { get; set; }

    }
}
