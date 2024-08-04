using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BehaviorDesignPattern.StrategyPattern.Models.Entity
{
    public class Product
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Display(Name = "Ürün Stok Miktarı")]
        public int Stock { get; set; }
        public int UserId { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.DateTime)]
        public DateTime CreatedDate { get; set; }

    }
}
