using LocadoraVeiculos.Models;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Data
{
    public class LocadoraContext : DbContext
    {
        public LocadoraContext(DbContextOptions<LocadoraContext> options)
            : base(options)
        {
        }

        public DbSet<Fabricante> Fabricantes { get; set; }
        public DbSet<CategoriaVeiculo> CategoriasVeiculos { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Aluguel> Alugueis { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.CPF)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Veiculo>()
                .HasIndex(v => v.Placa)
                .IsUnique();

            modelBuilder.Entity<Fabricante>()
                .HasIndex(f => f.Nome)
                .IsUnique();

            modelBuilder.Entity<CategoriaVeiculo>()
                .HasIndex(c => c.Nome)
                .IsUnique();

            modelBuilder.Entity<Veiculo>()
                .HasOne(v => v.Fabricante)
                .WithMany(f => f.Veiculos)
                .HasForeignKey(v => v.FabricanteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Veiculo>()
                .HasOne(v => v.CategoriaVeiculo)
                .WithMany(c => c.Veiculos)
                .HasForeignKey(v => v.CategoriaVeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Aluguel>()
                .HasOne(a => a.Cliente)
                .WithMany(c => c.Alugueis)
                .HasForeignKey(a => a.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Aluguel>()
                .HasOne(a => a.Veiculo)
                .WithMany(v => v.Alugueis)
                .HasForeignKey(a => a.VeiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Aluguel>()
                .Property(a => a.ValorDiaria)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Aluguel>()
                .Property(a => a.ValorTotal)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Veiculo>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Veiculo_Quilometragem",
                        "[Quilometragem] >= 0");
                });

            modelBuilder.Entity<Aluguel>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Aluguel_Datas",
                        "[DataFimPrevista] > [DataInicio]");

                    t.HasCheckConstraint(
                        "CK_Aluguel_Quilometragem",
                        "[QuilometragemFinal] IS NULL OR [QuilometragemFinal] >= [QuilometragemInicial]");

                    t.HasCheckConstraint(
                        "CK_Aluguel_ValorDiaria",
                        "[ValorDiaria] > 0");

                    t.HasCheckConstraint(
                        "CK_Aluguel_ValorTotal",
                        "[ValorTotal] >= 0");
                });
        }
    }
}