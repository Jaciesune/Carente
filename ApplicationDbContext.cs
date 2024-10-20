using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Dodaj DbSet dla każdej tabeli w bazie danych
    public DbSet<Uzytkownik> Uzytkownik { get; set; }  
}