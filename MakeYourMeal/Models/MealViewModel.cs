using System.Collections.Generic;
using System.Web.Mvc;

namespace BaBoMaZso.MakeYourMeal.Models
{
    /// <summary>
    /// Nézetmodell a "Make Your Own Meal" léptetős űrlaphoz.
    /// </summary>
    public class MealViewModel
    {
        public int ModuleId { get; set; }

        // Felhasználói választások
        public string SelectedPasta { get; set; }
        public string SelectedSauce { get; set; }
        public List<string> SelectedToppings1 { get; set; } = new List<string>();
        public List<string> SelectedToppings2 { get; set; } = new List<string>();
        public List<string> SelectedExtras { get; set; } = new List<string>();

        // Opciók listái (Hotcakes-ből töltött ViewBag helyett itt)
        public IEnumerable<SelectListItem> Pastas { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Sauces { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Toppings1 { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Toppings2 { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Extras { get; set; } = new List<SelectListItem>();
    }
}
