using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarfinaLibrary.Models
{
    [Table("products_marfina", Schema = "app")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("article")]
        public string Article { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("min_partner_price", TypeName = "decimal(10,2)")]
        public decimal MinPartnerPrice { get; set; }

        // Навигационное свойство
        public virtual ICollection<SalesHistory> SalesHistories { get; set; } = new List<SalesHistory>();
    }
}