namespace Carente.Models
{
    public class Car
    {
        public int Id { get; set; } // Klucz główny

        public string Typ_Silnika { get; set; } // Typ silnika
        public string Marka { get; set; } // Marka samochodu
        public string Model { get; set; } // Model samochodu
        public string Nr_Vin { get; set; } // Numer VIN
        public int Ilosc_Drzwi { get; set; } // Ilość drzwi
        public float Pojemnosc_Silnika { get; set; } // Pojemność silnika
        public string Kolor { get; set; } // Kolor samochodu
        public int Ilosc_Miejsc { get; set; } // Ilość miejsc
        public int Rocznik { get; set; } // Rok produkcji
        public string Status { get; set; } // Status samochodu

        public int? Oferta_Id { get; set; }
        public virtual Offer Oferta { get; set; } // Połączenie z ofertą
    }
}
