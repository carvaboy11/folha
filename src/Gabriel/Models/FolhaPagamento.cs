using System;

namespace Gabriel.Models
{
    public class FolhaPagamento
    {
        public int Id { get; set; }
        public int FuncionarioId { get; set; }
        public Funcionario? Funcionario { get; set; }

        public int Mes { get; set; }
        public int Ano { get; set; }

        // Valores calculados
        public decimal SalarioBruto { get; set; }
        public decimal DescontoIR { get; set; }
        public decimal DescontoINSS { get; set; }
        public decimal ValorFGTS { get; set; }
        public decimal SalarioLiquido { get; set; }
    }
}
