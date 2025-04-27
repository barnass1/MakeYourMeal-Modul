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
using System.Collections.Generic;
using System.Web.Mvc;

namespace BaBoMaZso.MakeYourMeal.Models
{
    public class Settings
    {
        public bool Setting1 { get; set; } = false;
        public DateTime Setting2 { get; set; } = DateTime.Now;
        public int MaxExtras { get; set; } = 5;
        public decimal ExtraPriceMultiplier { get; set; } = 1.0m;
        public string EnabledCategorySlugs { get; set; } = "pastas,sauces,toppings1,toppings2,extras";
        public string NoticeText { get; set; } = "Kérjük, válasszon legalább egy feltétet!";
        public List<string> DefaultSauceOptions { get; set; } = new List<string>();
    }
}