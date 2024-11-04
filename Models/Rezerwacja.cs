namespace Carente.Models
{
    public class Rezerwacja
    {
        public int Id { get; set; }
        public int Samochod_Id { get; set; }
        public int Uzytkownik_Id { get; set; }
        public DateTime Data_Rozpoczecia { get; set; }
        public DateTime Data_Zakonczenia { get; set; }

    }
}