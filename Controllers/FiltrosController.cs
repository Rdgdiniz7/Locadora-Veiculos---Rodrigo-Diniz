using LocadoraVeiculos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FiltrosController : ControllerBase
    {
        private readonly LocadoraContext _context;

        public FiltrosController(LocadoraContext context)
        {
            _context = context;
        }

        // FILTRO 1
        // INNER JOIN Veiculo + Fabricante

        [HttpGet("veiculos/fabricante/{nome}")]
        public async Task<IActionResult> VeiculosPorFabricante(string nome)
        {
            var resultado =
                await (
                    from v in _context.Veiculos
                    join f in _context.Fabricantes
                        on v.FabricanteId equals f.Id
                    where f.Nome.Contains(nome)
                    select new
                    {
                        v.Id,
                        v.Placa,
                        v.Modelo,
                        v.AnoFabricacao,
                        v.Quilometragem,
                        Fabricante = f.Nome
                    }
                ).ToListAsync();

            return Ok(resultado);
        }

        // FILTRO 2
        // INNER JOIN Veiculo + Categoria

        [HttpGet("veiculos/categoria/{nome}")]
        public async Task<IActionResult> VeiculosPorCategoria(string nome)
        {
            var resultado =
                await (
                    from v in _context.Veiculos
                    join c in _context.CategoriasVeiculos
                        on v.CategoriaVeiculoId equals c.Id
                    where c.Nome.Contains(nome)
                    select new
                    {
                        v.Id,
                        v.Placa,
                        v.Modelo,
                        Categoria = c.Nome,
                        v.Disponivel
                    }
                ).ToListAsync();

            return Ok(resultado);
        }

        // FILTRO 3
        // INNER JOIN Aluguel + Cliente + Veiculo

        [HttpGet("alugueis/cliente/{cpf}")]
        public async Task<IActionResult> AlugueisPorCliente(string cpf)
        {
            var resultado =
                await (
                    from a in _context.Alugueis
                    join c in _context.Clientes
                        on a.ClienteId equals c.Id
                    join v in _context.Veiculos
                        on a.VeiculoId equals v.Id
                    where c.CPF == cpf
                    select new
                    {
                        AluguelId = a.Id,
                        Cliente = c.Nome,
                        c.CPF,
                        Veiculo = v.Modelo,
                        v.Placa,
                        a.DataInicio,
                        a.DataFimPrevista,
                        a.DataDevolucao,
                        a.ValorTotal,
                        a.Status
                    }
                ).ToListAsync();

            return Ok(resultado);
        }

        // FILTRO 4
        // INNER JOIN Aluguel + Cliente + Veiculo
        // Filtrando período

        [HttpGet("alugueis/periodo")]
        public async Task<IActionResult> AlugueisPorPeriodo(
            DateTime inicio,
            DateTime fim)
        {
            if (fim < inicio)
            {
                return BadRequest(new
                {
                    mensagem = "A data final deve ser maior ou igual à inicial."
                });
            }

            var resultado =
                await (
                    from a in _context.Alugueis
                    join c in _context.Clientes
                        on a.ClienteId equals c.Id
                    join v in _context.Veiculos
                        on a.VeiculoId equals v.Id
                    where a.DataInicio >= inicio
                       && a.DataInicio <= fim
                    select new
                    {
                        AluguelId = a.Id,
                        Cliente = c.Nome,
                        Veiculo = v.Modelo,
                        v.Placa,
                        a.DataInicio,
                        a.DataFimPrevista,
                        a.ValorTotal,
                        a.Status
                    }
                ).ToListAsync();

            return Ok(resultado);
        }

        // FILTRO 5
        // LEFT JOIN Veiculo + Aluguel
        // Mostra veículos que nunca foram alugados

        [HttpGet("veiculos/nunca-alugados")]
        public async Task<IActionResult> VeiculosNuncaAlugados()
        {
            var resultado =
                await (
                    from v in _context.Veiculos

                    join a in _context.Alugueis
                        on v.Id equals a.VeiculoId
                        into alugueis

                    from a in alugueis.DefaultIfEmpty()

                    where a == null

                    select new
                    {
                        v.Id,
                        v.Placa,
                        v.Modelo,
                        v.AnoFabricacao,
                        v.Quilometragem,
                        v.Disponivel
                    }
                ).ToListAsync();

            return Ok(resultado);
        }
    }
}