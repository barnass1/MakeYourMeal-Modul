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
        public ActionResult AddToCartAjax(MealViewModel model)
        {
            try
            {
                var cart = _hcc.OrderServices.EnsureShoppingCart();

                AddProductToCart(model.SelectedPasta, cart);
                AddProductToCart(model.SelectedSauce, cart);
                AddProductsToCart(ParseSelectedItems(model.SelectedToppings1), cart);
                AddProductsToCart(ParseSelectedItems(model.SelectedToppings2), cart);
                AddProductsToCart(ParseSelectedItems(model.SelectedExtras), cart);

                _hcc.OrderServices.Orders.Update(cart);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void AddProductToCart(string bvin, Order cart)
        {
            if (string.IsNullOrWhiteSpace(bvin))
                return;

            var product = _hcc.CatalogServices.Products.Find(bvin);
            if (product == null)
                return;

            var lineItem = new LineItem
            {
                ProductId = product.Bvin,
                ProductName = product.ProductName,
                Quantity = 1,
                BasePricePerItem = product.SitePrice,
                LineTotal = product.SitePrice,
                ProductShortDescription = product.ShortDescription
            };

            _hcc.OrderServices.AddItemToOrder(cart, lineItem);
        }


        private void AddProductsToCart(IEnumerable<string> bvins, Order cart)
        {
            if (bvins == null)
                return;

            foreach (var bvin in bvins)
            {
                AddProductToCart(bvin, cart);
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
