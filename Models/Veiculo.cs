using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculos.Models
{
    public class Veiculo
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Placa { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Modelo { get; set; } = string.Empty;

        [Required]
        public int AnoFabricacao { get; set; }

        [Required]
        public int Quilometragem { get; set; }

        public bool Disponivel { get; set; } = true;

        public int FabricanteId { get; set; }

        public Fabricante? Fabricante { get; set; }

        public int CategoriaVeiculoId { get; set; }

        public CategoriaVeiculo? CategoriaVeiculo { get; set; }

        public ICollection<Aluguel> Alugueis { get; set; } = new List<Aluguel>();
    }
}