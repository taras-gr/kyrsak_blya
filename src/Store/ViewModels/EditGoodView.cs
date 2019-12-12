using System.Collections.Generic;
using DAL.Models;

namespace Store.ViewModels
{
    public class EditGoodView
    {
        public EditGoodView()
        {
            Storages = new List<Storage>();
            Producers = new List<Producer>();
            AllStorages = new List<Storage>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Specification { get; set; }

        public string PhotoUrl { get; set; }

        public int YearOfManufacture { get; set; }

        public int WarrantyTerm { get; set; }

        public Producer Producer { get; set; }

        public List<Producer> Producers { get; set; }

        public decimal Price { get; set; }

        public string Type { get; set; }

        public int Count { get; set; }

        public ICollection<Storage> AllStorages { get; set; }

        public ICollection<Storage> Storages { get; set; }
    }
}
