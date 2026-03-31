using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarfinaLibrary.Models
{
    [Table("partner_types_marfina", Schema = "app")]
    public class PartnerType
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string Name { get; set; }

        // Навигационное свойство
        public virtual ICollection<Partner> Partners { get; set; } = new List<Partner>();
    }
}