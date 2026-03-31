using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarfinaLibrary.Models
{
    [Table("partners_marfina", Schema = "app")]
    public class Partner
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("type_id")]
        public int TypeId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; }

        [Column("legal_address")]
        public string LegalAddress { get; set; }

        [MaxLength(12)]
        [Column("inn")]
        public string Inn { get; set; }

        [MaxLength(200)]
        [Column("director_name")]
        public string DirectorName { get; set; }

        [MaxLength(20)]
        [Column("phone")]
        public string Phone { get; set; }

        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("rating")]
        public int Rating { get; set; }

        [MaxLength(500)]
        [Column("logo_path")]
        public string LogoPath { get; set; }

        // Навигационные свойства
        [ForeignKey("TypeId")]
        public virtual PartnerType PartnerType { get; set; }

        public virtual ICollection<SalesHistory> SalesHistories { get; set; } = new List<SalesHistory>();
    }
}