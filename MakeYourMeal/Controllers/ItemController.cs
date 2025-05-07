using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BaBoMaZso.MakeYourMeal.Models;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using DotNetNuke.Entities.Portals;
using Hotcakes.Commerce.Extensions;

namespace BaBoMaZso.MakeYourMeal.Controllers
{
    [DnnHandleError]
    public class ItemController : DnnController
    {
        private readonly HotcakesApplication _hcc = HotcakesApplication.Current;

        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public ActionResult Assemble()
        {
            try
            {
                var model = new MealViewModel
                {
                    Pastas = LoadOptions("pastas"),
                    Sauces = LoadOptions("sauces"),
                    Toppings1 = LoadOptions("toppings1"),
                    Toppings2 = LoadOptions("toppings2"),
                    Extras = LoadOptions("extras")
                };

                return View("Assemble", model);
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return View("Error", new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult AddToCartAjax(MealViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Érvénytelen adatok." });
                }

                var cart = _hcc.OrderServices.EnsureShoppingCart();

                // Alaptermék SKU alapján
                var baseProduct = _hcc.CatalogServices.Products.FindBySku("ABC123"); // Cseréld le a saját SKU-ra
                if (baseProduct == null)
                {
                    return Json(new { success = false, message = "Nem található az alap ételtermék." });
                }

                var ingredientNames = new List<string>();
                decimal totalPrice = 0;

                void Add(string bvin)
                {
                    if (string.IsNullOrWhiteSpace(bvin)) return;
                    var p = _hcc.CatalogServices.Products.Find(bvin);
                    if (p != null)
                    {
                        ingredientNames.Add(p.ProductName);
                        totalPrice += p.SitePrice;
                    }
                }

                void AddMany(IEnumerable<string> bvins)
                {
                    foreach (var bvin in bvins) Add(bvin);
                }

                Add(model.SelectedPasta);
                Add(model.SelectedSauce);
                AddMany(ParseSelectedItems(model.SelectedToppings1));
                AddMany(ParseSelectedItems(model.SelectedToppings2));
                AddMany(ParseSelectedItems(model.SelectedExtras));

                var summary = string.Join(" + ", ingredientNames);

                var lineItem = baseProduct.ConvertToLineItem(_hcc, 1, new OptionSelections());
                lineItem.ProductName += " – " + summary;
                lineItem.ProductShortDescription = summary;
                lineItem.BasePricePerItem = totalPrice;
                lineItem.LineTotal = totalPrice;

                _hcc.AddToOrderWithCalculateAndSave(cart, lineItem);

                return Json(new { success = true, message = "Termék sikeresen kosárba helyezve." });
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return Json(new { success = false, message = "Hiba történt: " + ex.Message });
            }
        }

        private IEnumerable<string> ParseSelectedItems(string csvItems)
        {
            if (string.IsNullOrWhiteSpace(csvItems))
                return Enumerable.Empty<string>();

            return csvItems.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => x.Trim());
        }

        private List<ProductOptionViewModel> LoadOptions(string categorySlug)
        {
            var category = _hcc.CatalogServices.Categories.FindBySlug(categorySlug);
            if (category == null)
                return new List<ProductOptionViewModel>();

            int total = 0;
            var products = _hcc.CatalogServices.Products.FindByCriteria(new ProductSearchCriteria
            {
                CategoryId = category.Bvin,
                DisplayInactiveProducts = true
            }, 1, int.MaxValue, ref total);

            return products.Select(p => new ProductOptionViewModel
            {
                Name = p.ProductName,
                Value = p.Bvin,
                ImageUrl = GetProductImageUrl(p)
            }).ToList();
        }

        private string GetProductImageUrl(Product product)
        {
            if (product == null || string.IsNullOrEmpty(product.ImageFileMedium))
                return "";

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority).TrimEnd('/');
            var portalId = PortalSettings.Current.PortalId;
            var path = $"/Portals/{portalId}/Hotcakes/Data/products/{product.Bvin}/medium/{product.ImageFileMedium}";

            return baseUrl + path;
        }
    }
}