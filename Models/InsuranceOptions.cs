using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Carente.Models
{
    public class Ubezpieczenie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Wartosc { get; set; }

        [Required]
        [StringLength(45)]
        public string Typ { get; set; }

        [Required]
        [StringLength(45)]
        public string Opis { get; set; }

        [Required]
        public int Rezerwacja_Id { get; set; }

        // Navigation property for the associated reservation
        [ForeignKey("Rezerwacja_Id")]
        public Rezerwacja Rezerwacja { get; set; }
    }
}
