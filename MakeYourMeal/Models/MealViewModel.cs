using System.Collections.Generic;

namespace BaBoMaZso.MakeYourMeal.Models
{
    public class MealViewModel
    {
        public int ModuleId { get; set; }
        public string SelectedPasta { get; set; }
        public string SelectedSauce { get; set; }
        public List<string> SelectedToppings1 { get; set; } = new List<string>();
        public List<string> SelectedToppings2 { get; set; } = new List<string>();
        public List<string> SelectedExtras { get; set; } = new List<string>();
        public List<ProductOptionViewModel> Pastas { get; set; } = new List<ProductOptionViewModel>();
        public List<ProductOptionViewModel> Sauces { get; set; } = new List<ProductOptionViewModel>();
        public List<ProductOptionViewModel> Toppings1 { get; set; } = new List<ProductOptionViewModel>();
        public List<ProductOptionViewModel> Toppings2 { get; set; } = new List<ProductOptionViewModel>();
        public List<ProductOptionViewModel> Extras { get; set; } = new List<ProductOptionViewModel>();
    }
}