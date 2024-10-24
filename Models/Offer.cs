using System.ComponentModel.DataAnnotations.Schema;

public class Offer
{
    public int Id { get; set; }
    public float Cena { get; set; }

    // Uczyń 'Ocena' nullable
    public int? Ocena { get; set; }
    public string Opis { get; set; }
    public string Status { get; set; } = "Dostępna"; // Ustaw domyślną wartość
    public int? Rezerwacja_Id { get; set; } // Uczyń 'Rezerwacja_Id' nullable

    [NotMapped]
    public int SelectedCarId { get; set; } // Dodaj tę właściwość
}
