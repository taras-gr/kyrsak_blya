using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DAL.Models
{
    /// <summary>
    /// User model for create entity in database.
    /// </summary>
    public class Customer
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public Cart Cart { get; set; }

        public ICollection<GoodReview> Reviews { get; set; }
    }
}
