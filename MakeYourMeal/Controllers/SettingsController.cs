/*
' Copyright (c) 2025 Wok and Roll
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Web.Mvc;
using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;

namespace BaBoMaZso.MakeYourMeal.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : DnnController
    {
        [HttpGet]
        public ActionResult Settings()
        {
            var settings = new Models.Settings
            {
                Setting1 = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MakeYourMeal_Setting1", false),
                Setting2 = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MakeYourMeal_Setting2", DateTime.Now),
                MaxExtras = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MakeYourMeal_MaxExtras", 5),
                ExtraPriceMultiplier = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MakeYourMeal_ExtraPriceMultiplier", 1.0m),
                EnabledCategorySlugs = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MakeYourMeal_EnabledCategorySlugs", "pastas,sauces,toppings1,toppings2,extras"),
                NoticeText = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault("MakeYourMeal_NoticeText", "Kérjük, válasszon legalább egy feltétet!")
            };

            return View(settings);
        }

        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(Models.Settings settings)
        {
            ModuleContext.Configuration.ModuleSettings["MakeYourMeal_Setting1"] = settings.Setting1.ToString();
            ModuleContext.Configuration.ModuleSettings["MakeYourMeal_Setting2"] = settings.Setting2.ToUniversalTime().ToString("u");
            ModuleContext.Configuration.ModuleSettings["MakeYourMeal_MaxExtras"] = settings.MaxExtras.ToString();
            ModuleContext.Configuration.ModuleSettings["MakeYourMeal_ExtraPriceMultiplier"] = settings.ExtraPriceMultiplier.ToString();
            ModuleContext.Configuration.ModuleSettings["MakeYourMeal_EnabledCategorySlugs"] = settings.EnabledCategorySlugs;
            ModuleContext.Configuration.ModuleSettings["MakeYourMeal_NoticeText"] = settings.NoticeText;

            return RedirectToDefaultRoute();
        }
    }
}