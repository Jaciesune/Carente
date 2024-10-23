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

        // Klucz obcy do tabeli oferta
        public int Oferta_Id { get; set; }

        // Możesz również dodać nawigację do oferty, jeśli chcesz
        public virtual Offer Oferta { get; set; }
    }
}
