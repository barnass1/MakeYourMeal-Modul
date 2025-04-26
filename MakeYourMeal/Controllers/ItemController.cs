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
using DotNetNuke.Common; // Ezt hozzáadtam a NavigateURL miatt

namespace BaBoMaZso.MakeYourMeal.Controllers
{
    [DnnHandleError]
    public class ItemController : DnnController
    {
        private readonly HotcakesApplication _hcc = HotcakesApplication.Current;

        // --- ADMIN: Admin oldal megjelenítése ---
        [HttpGet]
        public ActionResult Admin()
        {
            var criteria = new ProductSearchCriteria
            {
                Keyword = "",
                DisplayInactiveProducts = true
            };

            int totalCount = 0;
            var products = _hcc.CatalogServices.Products.FindByCriteria(criteria, 1, 1000, ref totalCount);

            ViewBag.HotcakesProducts = products.Select(p => new SelectListItem
            {
                Text = p.ProductName,
                Value = p.Bvin
            }).ToList();

            var model = new ProductAdminViewModel();

            return View("Admin", model);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult SaveProduct(ProductAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var selected = (List<SelectedProductViewModel>)Session["SelectedProducts"];
                if (selected == null)
                {
                    selected = new List<SelectedProductViewModel>();
                }

                selected.Add(new SelectedProductViewModel
                {
                    Bvin = model.SelectedProductBvin,
                    CategorySlug = model.CategorySlug
                });

                Session["SelectedProducts"] = selected;

                TempData["Success"] = "Termék sikeresen hozzárendelve!";
                return Redirect(DotNetNuke.Common.Globals.NavigateURL(this.ModuleContext.TabId, "Admin", "mid=" + this.ModuleContext.ModuleId));
            }

            TempData["Error"] = "Hiba a termék mentése közben.";
            return Redirect(DotNetNuke.Common.Globals.NavigateURL(this.ModuleContext.TabId, "Admin", "mid=" + this.ModuleContext.ModuleId));
        }

        // --- VÁSÁRLÓ: Főoldal ---
        public ActionResult Index()
        {
            return View("Index");
        }

        // --- VÁSÁRLÓ: Összeállítás oldal ---
        [HttpGet]
        public ActionResult Assemble()
        {
            var selectedProducts = (List<SelectedProductViewModel>)Session["SelectedProducts"] ?? new List<SelectedProductViewModel>();

            var model = new MealViewModel
            {
                Pastas = LoadOptionsByCategory(selectedProducts, "pastas"),
                Sauces = LoadOptionsByCategory(selectedProducts, "sauces"),
                Toppings1 = LoadOptionsByCategory(selectedProducts, "toppings1"),
                Toppings2 = LoadOptionsByCategory(selectedProducts, "toppings2"),
                Extras = LoadOptionsByCategory(selectedProducts, "extras")
            };

            return View("Assemble", model);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Assemble(MealViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Assemble");
            }

            var baseSku = "custom-meal";
            var product = _hcc.CatalogServices.Products.FindBySlug(baseSku);
            if (product == null)
            {
                TempData["Error"] = "A 'custom-meal' nevű alap termék nem található.";
                return RedirectToDefaultRoute();
            }

            decimal extraCost = 0m;
            if (model.SelectedToppings1 != null && model.SelectedToppings1.Count > 1)
                extraCost += 500m;
            if (model.SelectedToppings2 != null && model.SelectedToppings2.Count > 1)
                extraCost += 200m;

            var finalPrice = product.SitePrice + extraCost;

            var description = $"Tészta: {GetProductName(model.SelectedPasta)}\n" +
                              $"Szósz: {GetProductName(model.SelectedSauce)}\n" +
                              $"Feltét1: {GetProductNames(model.SelectedToppings1)}\n" +
                              $"Feltét2: {GetProductNames(model.SelectedToppings2)}\n" +
                              $"Extrák: {GetProductNames(model.SelectedExtras)}";

            var cart = _hcc.OrderServices.EnsureShoppingCart();
            cart.Items.Add(new LineItem
            {
                ProductId = product.Bvin,
                ProductName = "Saját étel összeállítás",
                Quantity = 1,
                BasePricePerItem = finalPrice,
                LineTotal = finalPrice,
                ProductShortDescription = description,
                ShippingPortion = 0,
                TaxPortion = 0
            });

            _hcc.OrderServices.Orders.Upsert(cart);

            return Redirect("/DesktopModules/Hotcakes/Core/Admin/Cart/ViewCart.aspx");
        }

        // --- SEGÉDFÜGGVÉNYEK ---

        private List<SelectListItem> LoadOptionsByCategory(List<SelectedProductViewModel> selectedProducts, string categorySlug)
        {
            return selectedProducts
                .Where(p => p.CategorySlug == categorySlug)
                .Select(p =>
                {
                    var product = _hcc.CatalogServices.Products.Find(p.Bvin);
                    return new SelectListItem
                    {
                        Text = product?.ProductName ?? "Ismeretlen termék",
                        Value = p.Bvin
                    };
                })
                .ToList();
        }

        private string GetProductName(string bvin)
        {
            if (string.IsNullOrEmpty(bvin))
                return "Nincs kiválasztva";

            var product = _hcc.CatalogServices.Products.Find(bvin);
            return product?.ProductName ?? "Ismeretlen termék";
        }

        private string GetProductNames(IList<string> bvins)
        {
            if (bvins == null || !bvins.Any())
                return "Nincs kiválasztva";

            return string.Join(", ", bvins.Select(GetProductName));
        }
    }
}
