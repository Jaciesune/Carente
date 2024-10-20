using System.ComponentModel.DataAnnotations;

public class Register
{
    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków.")]
    public string imie { get; set; }

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków.")]
    public string nazwisko { get; set; }

    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public string email { get; set; }

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "Hasło musi mieć przynajmniej {2} znaki.", MinimumLength = 6)]
    public string haslo { get; set; }

    [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
    [Phone(ErrorMessage = "Niepoprawny format numeru telefonu.")]
    public string tel { get; set; }
}
