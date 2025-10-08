using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Gabriel.Models;

#nullable disable

namespace Gabriel.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Gabriel.Models.Funcionario", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<decimal>("ValorHora")
                    .HasColumnType("TEXT");

                b.Property<int>("HorasTrabalhadas")
                    .HasColumnType("INTEGER");

                b.Property<string>("Cpf")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.Property<string>("Nome")
                    .IsRequired()
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("Cpf")
                    .IsUnique();

                b.ToTable("Funcionarios");
            });

            modelBuilder.Entity("Gabriel.Models.FolhaPagamento", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("INTEGER");

                b.Property<int>("Ano")
                    .HasColumnType("INTEGER");

                b.Property<int>("FuncionarioId")
                    .HasColumnType("INTEGER");

                b.Property<int>("Mes")
                    .HasColumnType("INTEGER");

                b.Property<decimal>("SalarioBruto")
                    .HasColumnType("TEXT");

                b.Property<decimal>("SalarioLiquido")
                    .HasColumnType("TEXT");

                b.Property<decimal>("ValorFGTS")
                    .HasColumnType("TEXT");

                b.Property<decimal>("DescontoINSS")
                    .HasColumnType("TEXT");

                b.Property<decimal>("DescontoIR")
                    .HasColumnType("TEXT");

                b.HasKey("Id");

                b.HasIndex("FuncionarioId");

                b.ToTable("Folhas");
            });

            modelBuilder.Entity("Gabriel.Models.FolhaPagamento", b =>
            {
                b.HasOne("Gabriel.Models.Funcionario")
                    .WithMany()
                    .HasForeignKey("FuncionarioId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
