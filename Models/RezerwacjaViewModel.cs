namespace Carente.Models
{
    public class RezerwacjaViewModel
    {
        public int Id { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public decimal Cena { get; set; }
        public DateTime Data_Rozpoczecia { get; set; }
        public DateTime Data_Zakonczenia { get; set; }
        public string WybraneUbezpieczenie { get; set; }
    }
}
