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

        [HttpPost]
        public ActionResult AddToCartAjax(MealViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, message = "Érvénytelen adatok." });
                }

                // Példa: a szükséges adatok kinyerése a formból
                string productId = Request.Form["ProductId"];
                string productBvin = Request.Form["bvin"];
                string nev = Request.Form["ProductName"];

                // Kosárba rakás
                KosarbaRakas(productBvin, nev);

                // Kosár URL
                string kosarUrl = "http://" + DotNetNuke.Entities.Portals.PortalSettings.Current.PortalAlias.HTTPAlias + "/Cart/";

                return Json(new { success = true, redirectUrl = kosarUrl });
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
                return Json(new { success = false, message = "Hiba történt: " + ex.Message });
            }
        }

        public bool KosarbaRakas(string bvin, string nev)
        {

            var HccApp = HotcakesApplication.Current;
            if (HccApp == null)
            {
                return false;
            }

            if (HccApp.OrderServices == null)
            {
                return false;
            }

            Order order = HccApp.OrderServices.CurrentShoppingCart();

            if (order == null)
            {
                order = HccApp.OrderServices.EnsureShoppingCart();
            }

            var lineItem = new LineItem
            {
                ProductId = bvin,
                ProductName = nev,
            };

            bool result = HccApp.AddToOrderAndSave(order, lineItem);

            return result;
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