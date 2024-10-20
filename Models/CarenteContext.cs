﻿using Microsoft.EntityFrameworkCore;

public class CarenteContext : DbContext
{
    public CarenteContext(DbContextOptions<CarenteContext> options) : base(options) { }

    // DbSet dla tabeli Uzytkownik
    public DbSet<Uzytkownik> Uzytkownik { get; set; }
}
