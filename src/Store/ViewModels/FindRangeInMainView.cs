using System.Collections.Generic;
using System.Linq;
using DAL.Classes;
using DAL.Classes.UnitOfWork;
using DAL.Models;

namespace Store.ViewModels
{
    public class FindRangeInMainView
    {
        public FindRangeInMainView()
        {
            this.Types = new List<string>
            {
                "All"
            };

            AllSort = new Dictionary<string, string>
            {
                { SortBy.PriceFromLower.ToString(), "Найнижча ціна" },
                { SortBy.PriceFromBigger.ToString(), "Найвища ціна" },
                { SortBy.Popularity.ToString(), "Найбільш популярні" },
            };
        }

        public FindRangeInMainView(UnitOfWork unitOfWork) : this()
        {
            var allTypes = unitOfWork.Goods.GetAll()
                .Select(p => p.Type)
                .Distinct();
            this.Types.AddRange(allTypes);
        }

        public FindGoodView GoodView { get; set; }

        public IEnumerable<Good> Goods { get; set; }

        public List<string> Types { get; set; }

        public string ChoosenType { get; set; }

        public SortBy SortBy { get; set; }

        public Dictionary<string, string> AllSort { get; set; }
    }
}
