using System.ComponentModel.DataAnnotations;

namespace LocadoraVeiculos.Models
{
    public class Fabricante
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? PaisOrigem { get; set; }

        public ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
    }
}