using System.ComponentModel.DataAnnotations;

public class Uzytkownik
{
    public int id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(50, ErrorMessage = "Imię nie może mieć więcej niż 50 znaków.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Imię może zawierać tylko litery.")]
    public string imie { get; set; }

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(50, ErrorMessage = "Nazwisko nie może mieć więcej niż 50 znaków.")]
    [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Nazwisko może zawierać tylko litery.")]
    public string nazwisko { get; set; }

    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email.")]
    public string email { get; set; }

    [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
    [Phone(ErrorMessage = "Nieprawidłowy format numeru telefonu.")]
    public string tel { get; set; }

    // Hasło nie jest wymagane podczas edycji
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć od 6 do 100 znaków.")]
    public string haslo { get; set; }

    public bool typ { get; set; }
}
