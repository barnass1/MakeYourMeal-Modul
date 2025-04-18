/*
' Copyright (c) 2025 Wok and Roll
' All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BaBoMaZso.MakeYourMeal.Models;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;

namespace BaBoMaZso.MakeYourMeal.Controllers
{
    [DnnHandleError]
    public class ItemController : DnnController
    {
        private readonly HotcakesApplication _hcc = HotcakesApplication.Current;

        // Segéd: lekérhető terméklista egy adott kategória slug alapján
        private IEnumerable<SelectListItem> GetOptionsByCategorySlug(string slug)
        {
            var category = _hcc.CatalogServices.Categories.FindBySlug(slug);
            if (category == null) return Enumerable.Empty<SelectListItem>();

            var criteria = new ProductSearchCriteria { CategoryId = category.Bvin };
            int totalCount = 0;
            var products = _hcc.CatalogServices.Products.FindByCriteria(criteria, 1, 100, ref totalCount);

            return products.Select(p => new SelectListItem
            {
                Text = p.ProductName,
                Value = p.Bvin
            });
        }

        // ─────────────────────────────────────────────
        // Nyitó nézet (csak új rendelés gomb, üres lista)
        // ─────────────────────────────────────────────
        [ModuleAction(ControlKey = "AddItem", TitleKey = "AddItem")]
        public ActionResult Index()
        {
            var items = new List<Item>(); // nem használunk adatbázist most
            return View("Index", items);
        }

        // ─────────────────────────────────────────────
        // Összerakó nézet (GET)
        // ─────────────────────────────────────────────
        public ActionResult Assemble()
        {
            var model = new MealViewModel
            {
                ModuleId = ModuleContext.ModuleId,
                Pastas = GetOptionsByCategorySlug("pastas"),
                Sauces = GetOptionsByCategorySlug("sauces"),
                Toppings1 = GetOptionsByCategorySlug("toppings1"),
                Toppings2 = GetOptionsByCategorySlug("toppings2"),
                Extras = GetOptionsByCategorySlug("extras")
            };

            return View("Assemble", model);
        }

        // ─────────────────────────────────────────────
        // Összerakó (POST) – Kosárba tesz
        // ─────────────────────────────────────────────
        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Assemble(MealViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Pastas = GetOptionsByCategorySlug("pastas");
                model.Sauces = GetOptionsByCategorySlug("sauces");
                model.Toppings1 = GetOptionsByCategorySlug("toppings1");
                model.Toppings2 = GetOptionsByCategorySlug("toppings2");
                model.Extras = GetOptionsByCategorySlug("extras");
                return View("Assemble", model);
            }

            // Leírás összeállítása
            var description = $"Tészta: {model.SelectedPasta}, Szósz: {model.SelectedSauce}\n" +
                              $"F1: {string.Join(", ", model.SelectedToppings1)}\n" +
                              $"F2: {string.Join(", ", model.SelectedToppings2)}\n" +
                              $"Extrák: {string.Join(", ", model.SelectedExtras)}";

            // Alaptermék betöltése slug alapján
            var baseSku = "custom-meal";
            var product = _hcc.CatalogServices.Products.FindBySlug(baseSku);
            if (product == null)
            {
                TempData["Error"] = "A 'custom-meal' nevű termék nem található. Kérjük, hozd létre a Hotcakes adminban.";
                return RedirectToDefaultRoute();
            }

            // Extra árak számítása
            decimal extraCost = 0m;
            if (model.SelectedToppings1 != null && model.SelectedToppings1.Count > 1)
                extraCost += 500m;
            if (model.SelectedToppings2 != null && model.SelectedToppings2.Count > 1)
                extraCost += 200m;

            var basePrice = product.SitePrice;
            var finalPrice = basePrice + extraCost;

            // Kosárba helyezés
            var cart = _hcc.OrderServices.EnsureShoppingCart();

            var lineItem = new LineItem
            {
                ProductId = product.Bvin,
                ProductName = "Saját étel összeállítás",
                Quantity = 1,
                BasePricePerItem = finalPrice,
                LineTotal = finalPrice,
                ProductShortDescription = description,
                ShippingPortion = 0,
                TaxPortion = 0
            };

            cart.Items.Add(lineItem);
            _hcc.OrderServices.Orders.Upsert(cart);

            // Kosár oldalra irányítás
            return Redirect("/DesktopModules/Hotcakes/Core/Admin/Cart/ViewCart.aspx");
        }
    }
}
