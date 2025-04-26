using System.Collections.Generic;
using System.Web.Mvc;

namespace BaBoMaZso.MakeYourMeal.Models
{
    public class ProductAdminViewModel
    {
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public string CategorySlug { get; set; }

        public List<SelectListItem> AvailableProducts { get; set; } = new List<SelectListItem>();

        public string SelectedProductBvin { get; set; }
    }
}
