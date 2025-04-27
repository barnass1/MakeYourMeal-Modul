using System.Collections.Generic;

namespace BaBoMaZso.MakeYourMeal.Models
{
    public class MealViewModel
    {
        public string SelectedPasta { get; set; }
        public string SelectedSauce { get; set; }
        public string SelectedToppings1 { get; set; }
        public string SelectedToppings2 { get; set; }
        public string SelectedExtras { get; set; }

        public IEnumerable<ProductOptionViewModel> Pastas { get; set; }
        public IEnumerable<ProductOptionViewModel> Sauces { get; set; }
        public IEnumerable<ProductOptionViewModel> Toppings1 { get; set; }
        public IEnumerable<ProductOptionViewModel> Toppings2 { get; set; }
        public IEnumerable<ProductOptionViewModel> Extras { get; set; }
    }

}