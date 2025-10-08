using System;

namespace Gabriel.Models
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public decimal ValorHora { get; set; }
        public int HorasTrabalhadas { get; set; }
    }
}
