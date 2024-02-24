using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EFIntro.Models;

public partial class MinionsDbContext : DbContext
{
    public MinionsDbContext()
    {
    }

    public MinionsDbContext(DbContextOptions<MinionsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<EvilnessFactor> EvilnessFactors { get; set; }

    public virtual DbSet<Minion> Minions { get; set; }

    public virtual DbSet<Town> Towns { get; set; }

    public virtual DbSet<Villain> Villains { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server= .\\SQLEXPRESS;Database=MinionsDB;Integrated Security=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Countrie__3214EC07163762B4");
        });

        modelBuilder.Entity<EvilnessFactor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Evilness__3214EC075FAE7B03");
        });

        modelBuilder.Entity<Minion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Minions__3214EC074CC0A2CA");

            entity.HasOne(d => d.Town).WithMany(p => p.Minions).HasConstraintName("FK__Minions__TownId__3B75D760");

            entity.HasMany(d => d.Villains).WithMany(p => p.Minions)
                .UsingEntity<Dictionary<string, object>>(
                    "MinionsVillain",
                    r => r.HasOne<Villain>().WithMany()
                        .HasForeignKey("VillainId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MinionsVi__Villa__440B1D61"),
                    l => l.HasOne<Minion>().WithMany()
                        .HasForeignKey("MinionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__MinionsVi__Minio__4316F928"),
                    j =>
                    {
                        j.HasKey("MinionId", "VillainId");
                        j.ToTable("MinionsVillains");
                    });
        });

        modelBuilder.Entity<Town>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Towns__3214EC0773AC622A");

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Towns).HasConstraintName("FK__Towns__CountryCo__38996AB5");
        });

        modelBuilder.Entity<Villain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Villains__3214EC07C628216D");

            entity.HasOne(d => d.EvilnessFactor).WithMany(p => p.Villains).HasConstraintName("FK__Villains__Evilne__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
