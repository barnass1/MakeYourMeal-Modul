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

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Assemble(MealViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Assemble");
            }

            var cart = _hcc.OrderServices.EnsureShoppingCart();

            AddToCart(model.SelectedPasta, cart);
            AddToCart(model.SelectedSauce, cart);
            AddMultipleToCart(model.SelectedToppings1, cart);
            AddMultipleToCart(model.SelectedToppings2, cart);
            AddMultipleToCart(model.SelectedExtras, cart);

            _hcc.OrderServices.Orders.Upsert(cart);

            return Redirect("/HotcakesStore/Cart");
        }


        private void AddToCart(string bvin, Order cart)
        {
            if (string.IsNullOrWhiteSpace(bvin)) return;

            var product = _hcc.CatalogServices.Products.Find(bvin);
            if (product == null) return;

            cart.Items.Add(new LineItem
            {
                ProductId = product.Bvin,
                ProductName = product.ProductName,
                Quantity = 1,
                BasePricePerItem = product.SitePrice,
                LineTotal = product.SitePrice,
                ProductShortDescription = product.ShortDescription
            });
        }

        private void AddMultipleToCart(IEnumerable<string> bvins, Order cart)
        {
            if (bvins == null) return;
            foreach (var bvin in bvins)
            {
                AddToCart(bvin, cart);
            }
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


        private string GetProductImageUrl(Product p)
        {
            if (p == null || string.IsNullOrEmpty(p.ImageFileMedium))
                return "";

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority).TrimEnd('/');
            var portalId = PortalSettings.PortalId; 
            var filename = p.ImageFileMedium;

            return $"{baseUrl}/Portals/{portalId}/Hotcakes/Data/products/{p.Bvin}/medium/{filename}";
        }


    }
}
