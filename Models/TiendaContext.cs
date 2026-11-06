using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TiendaVideojuegosApi.Models;

public partial class TiendaContext : DbContext
{
    public TiendaContext()
    {
    }

    public TiendaContext(DbContextOptions<TiendaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Se deja vacío porque la configuración se inyecta desde Program.cs leyendo User Secrets
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeo de Developer
        modelBuilder.Entity<Developer>(entity =>
        {
            entity.ToTable("developer");
            entity.HasKey(e => e.Id).HasName("developer_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Country).HasColumnName("country");
            entity.Property(e => e.FoundedYear).HasColumnName("founded_year");
        });

        // Mapeo de Genre
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.ToTable("genre");
            entity.HasKey(e => e.Id).HasName("genre_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        // Mapeo de Game
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("game");
            entity.HasKey(e => e.Id).HasName("game_pkey");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ReleaseDate).HasColumnName("release_date");
            entity.Property(e => e.AgeRating).HasColumnName("age_rating");
            entity.Property(e => e.KeyStock).HasColumnName("key_stock");
            entity.Property(e => e.DeveloperId).HasColumnName("developer_id");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");

            entity.HasOne(d => d.Developer)
                .WithMany(p => p.Games)
                .HasForeignKey(d => d.DeveloperId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("game_developer_id_fkey");

            entity.HasOne(d => d.Genre)
                .WithMany(p => p.Games)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("game_genre_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}