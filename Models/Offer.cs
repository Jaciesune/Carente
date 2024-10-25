using System.ComponentModel.DataAnnotations.Schema;

namespace Carente.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public float Cena { get; set; }
        public int? Ocena { get; set; }
        public string Opis { get; set; }
        public string Status { get; set; } = "Dostępna";
        public int? Rezerwacja_Id { get; set; }

        // Dodaj tę właściwość do modelu
        [NotMapped]
        public int SelectedCarId { get; set; } // Właściwość do przechowywania ID wybranego samochodu

    }
}
