using Microsoft.EntityFrameworkCore;

namespace Gabriel.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<FolhaPagamento> Folhas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Funcionario>().HasIndex(f => f.Cpf).IsUnique();
            modelBuilder.Entity<FolhaPagamento>()
                .HasOne(f => f.Funcionario)
                .WithMany()
                .HasForeignKey(f => f.FuncionarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
