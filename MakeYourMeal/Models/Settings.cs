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
        // Példa beállítás - aktiválja-e az "Assemble" funkciót
        public bool Setting1 { get; set; } = false;

        // Példa beállítás - meddig legyen érvényes az "Assemble" opció
        public DateTime Setting2 { get; set; } = DateTime.Now;

        // Új beállítás - alapértelmezett maximum extrák száma
        public int MaxExtras { get; set; } = 5;

        // Új beállítás - elérhető extrák árának százalékos felára (csak példa)
        public decimal ExtraPriceMultiplier { get; set; } = 1.0m;

        // Választható kategóriák slug listája, vesszővel elválasztva (opcionális)
        public string EnabledCategorySlugs { get; set; } = "pastas,sauces,toppings1,toppings2,extras";

        // UI-n megjelenített megjegyzés vagy figyelmeztetés
        public string NoticeText { get; set; } = "Kérjük, válasszon legalább egy feltétet!";

        // Lista az alapértelmezett választható szószokról (tömbként is értelmezhető)
        public List<string> DefaultSauceOptions { get; set; } = new List<string>();
    }
}