namespace BaBoMaZso.MakeYourMeal.Models
{
    using DotNetNuke.ComponentModel.DataAnnotations;
    using System.Web.Caching;
    using System;

    [TableName("MakeYourMeal_Items")]
    [PrimaryKey("ItemId", AutoIncrement = true)]
    [Cacheable("Items", CacheItemPriority.Default, 20)]
    [Scope("ModuleId")]
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public int AssignedUserId { get; set; }
        public int ModuleId { get; set; }
        public int CreatedByUserId { get; set; }
        public int LastModifiedByUserId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public DateTime LastModifiedOnDate { get; set; }

        // Új mezők a felhasználói választások tárolásához
        public string SelectedPasta { get; set; }
        public string SelectedSauce { get; set; }
        public string SelectedToppings1 { get; set; }
        public string SelectedToppings2 { get; set; }
        public string SelectedExtras { get; set; }
    }
}
