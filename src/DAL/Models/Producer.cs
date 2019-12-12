using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DAL.Models
{
    /// <summary>
    /// Producer model for create entity in database
    /// </summary>
    public class Producer
    {
        public Producer()
        {
            Products = new List<Good>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string WebSite { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public ICollection<Good> Products { get; set; }
    }
}
