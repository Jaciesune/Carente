namespace Carente.Models
{
    public class RezerwacjaViewModel
    {
        public int Samochod_Id { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public decimal Cena { get; set; }
        public DateTime Data_Rozpoczecia { get; set; }
        public DateTime Data_Zakonczenia { get; set; }
    }
}
