using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FOODPEDI.API.REST.DataAccess.Entities
{
    public class Item
    {
        [Column(Order = 0), Key]
        public string Id { get; set; }

        public ItemState State { get; set; }
        public string Name { get; set; }
        public string BrandId { get; set; }
        public Brand Brand { get; set; }
        public string ShortDescription1 { get; set; }
        public string ShortDescription2 { get; set; }
        public string ShortDescription3 { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string WebPageUrl1 { get; set; }
        public string WebPageUrl2 { get; set; }
        public string WebPageUrl3 { get; set; }
        public string WebPageUrl4 { get; set; }
        public string Barcode1 { get; set; }
        public string Barcode2 { get; set; }
        public string Barcode3 { get; set; }

        public double ItemWeight { get; set; }
        public int? MadeCountryId { get; set; }
        public int? MadeStateId { get; set; }
        public int? MadeCityId { get; set; }
        public MadeCountry MadeCountry { get; set; }
        public MadeState MadeState { get; set; }
        public MadeCity MadeCity { get; set; }

        public string ImageFolderPath { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }

        public List<ItemComment> ItemCommends { get; set; }
        public List<ItemCategory> ItemCategories { get; set; }
        public List<ItemImage> ItemImages { get; set; }
        public List<ItemIngredient> ItemIngredients { get; set; }
    }

   
    public enum ItemState:byte
    {
        Waiting=0,
        Confirmed=1,
        Unconfirmed=2
    }
    public class ItemCategory
    {
        [Column(Order = 0), Key]
        public string Id { get; set; }
        public string CategoryId { get; set; }
        public string ItemId { get; set; }
        public Category Category { get; set; }
        public Item Item { get; set; }
    }
    public class ItemIngredient
    {
        [Column(Order = 0), Key]
        public string Id { get; set; }
        public string IngredientId { get; set; }
        public string ItemId { get; set; }
        public double Value { get; set; }
        public string UnitCode { get; set; }
        public Item Item { get; set; }
        public Ingredient Ingredient { get; set; }

    }

    public class ItemImage
    {
        [Column(Order = 0), Key]
        public string Id { get; set; }
        public string ItemId { get; set; }
        public bool IsMainImage { get; set; }
        public short OrderNumber { get; set; }

        public Item Item { get; set; }

    }

    public class ItemComment
    {
        [Column(Order = 0), Key]
        public string Id { get; set; }
        public string ItemId { get; set; }
        public bool IsPassive { get; set; }
        public string ParentId { get; set; }
        public string UserId { get; set; }
        public bool ShowUser { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public Item Item { get; set; }
        public List<ItemComment> ChildCommends { get; set; }
        public ItemComment Parent { get; set; }
        public AppUser User { get; set; }


    }
}
