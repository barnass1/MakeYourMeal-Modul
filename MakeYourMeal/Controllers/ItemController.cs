/*
' Copyright (c) 2025 Wok and Roll
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BaBoMaZso.MakeYourMeal.Components;
using BaBoMaZso.MakeYourMeal.Models;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using Hotcakes.Commerce;                
using Hotcakes.Commerce.Catalog;    

namespace BaBoMaZso.MakeYourMeal.Controllers
{
    [DnnHandleError]
    public class ItemController : DnnController
    {
        private readonly HotcakesApplication HccApp = HotcakesApplication.Current; 
        private List<SelectListItem> GetToppingsFromHotcakes()
        {
            string toppingsCategoryId = "ac43dc33-306b-4fed-b905-afa01c66ac0d";

            var criteria = new ProductSearchCriteria
            {
                CategoryId = toppingsCategoryId
            };

            int totalCount = 0;
            var toppings = HccApp.CatalogServices.Products.FindByCriteria(criteria, 1, int.MaxValue, ref totalCount);

            var selectList = toppings.Select(p => new SelectListItem
            {
                Text = p.ProductName,
                Value = p.Bvin
            }).ToList();

            return selectList;
        }

        public ActionResult Delete(int itemId)
        {
            ItemManager.Instance.DeleteItem(itemId, ModuleContext.ModuleId);
            return RedirectToDefaultRoute();
        }

        public ActionResult Edit(int itemId = -1)
        {
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(CommonJs.DnnPlugins);
            var userlist = UserController.GetUsers(PortalSettings.PortalId);
            var users = from user in userlist.Cast<UserInfo>().ToList()
                        select new SelectListItem { Text = user.DisplayName, Value = user.UserID.ToString() };
            ViewBag.Users = users;
            ViewBag.Toppings = GetToppingsFromHotcakes();
            var item = (itemId == -1)
                 ? new Item { ModuleId = ModuleContext.ModuleId }
                 : ItemManager.Instance.GetItem(itemId, ModuleContext.ModuleId);

            return View(item);
        }

        [HttpPost]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Edit(Item item)
        {
            if (item.ItemId == -1)
            {
                item.CreatedByUserId = User.UserID;
                item.CreatedOnDate = DateTime.UtcNow;
                item.LastModifiedByUserId = User.UserID;
                item.LastModifiedOnDate = DateTime.UtcNow;

                ItemManager.Instance.CreateItem(item);
            }
            else
            {
                var existingItem = ItemManager.Instance.GetItem(item.ItemId, item.ModuleId);
                existingItem.LastModifiedByUserId = User.UserID;
                existingItem.LastModifiedOnDate = DateTime.UtcNow;
                existingItem.ItemName = item.ItemName;
                existingItem.ItemDescription = item.ItemDescription;
                existingItem.AssignedUserId = item.AssignedUserId;

                ItemManager.Instance.UpdateItem(existingItem);
            }

            return RedirectToDefaultRoute();
        }

        [ModuleAction(ControlKey = "Edit", TitleKey = "AddItem")]
        public ActionResult Index()
        {
            var items = ItemManager.Instance.GetItems(ModuleContext.ModuleId);
            return View(items);
        }

        // ───────────────────────────────────────────────────────────────────────────────
        // ÚJ: "Make Your Own Meal" összeállító felület
        // ───────────────────────────────────────────────────────────────────────────────

        public ActionResult Assemble()
        {
            // TODO: Később itt lekérheted a tészta, szósz, topping stb. opciókat
            return View();
        }

        [HttpPost]
        public ActionResult Assemble(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }

            item.ModuleId = ModuleContext.ModuleId;
            item.CreatedByUserId = User.UserID;
            item.CreatedOnDate = DateTime.UtcNow;
            item.LastModifiedByUserId = User.UserID;
            item.LastModifiedOnDate = DateTime.UtcNow;

            ItemManager.Instance.CreateItem(item);
            return RedirectToDefaultRoute();
        }
    }
}
