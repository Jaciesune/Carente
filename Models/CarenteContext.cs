using Carente.Models;
using Microsoft.EntityFrameworkCore;

public class CarenteContext : DbContext
{
    public CarenteContext(DbContextOptions<CarenteContext> options) : base(options) { }

    // DbSet dla tabeli Uzytkownik
    public DbSet<Uzytkownik> Uzytkownik { get; set; }

    // DbSet dla tabeli Oferta
    public DbSet<Offer> Oferta { get; set; }

    public DbSet<Rezerwacja> Rezerwacja { get; set; }


    // DbSet dla tabeli Samochod
    public DbSet<Car> Samochod { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfiguracja relacji między Car a Offer
        modelBuilder.Entity<Car>()
            .HasOne(c => c.Oferta) // Samochód ma jedną ofertę
            .WithOne() // Oferta ma jeden samochód
            .HasForeignKey<Car>(c => c.Oferta_Id) // Użyj klucza obcego Oferta_Id w tabeli Samochod
            .OnDelete(DeleteBehavior.Cascade); // Kaskadowe usunięcie, jeśli oferta zostanie usunięta

        // Inne konfiguracje modelu mogą być dodane tutaj
    }
}
