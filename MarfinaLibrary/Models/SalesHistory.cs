using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarfinaLibrary.Models
{
    [Table("sales_history_marfina", Schema = "app")]
    public class SalesHistory
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("partner_id")]
        public int PartnerId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [Column("sale_date")]
        public DateTime SaleDate { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("total_amount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        // Навигационные свойства
        [ForeignKey("PartnerId")]
        public virtual Partner Partner { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}