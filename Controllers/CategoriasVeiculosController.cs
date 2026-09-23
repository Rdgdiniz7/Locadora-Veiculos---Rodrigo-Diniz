using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculosController : ControllerBase
    {
        private readonly LocadoraContext _context;

        public VeiculosController(LocadoraContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var veiculos = await _context.Veiculos
                .Include(v => v.Fabricante)
                .Include(v => v.CategoriaVeiculo)
                .Select(v => new
                {
                    v.Id,
                    v.Placa,
                    v.Modelo,
                    v.AnoFabricacao,
                    v.Quilometragem,
                    v.Disponivel,

                    Fabricante = v.Fabricante != null
                        ? v.Fabricante.Nome
                        : null,

                    Categoria = v.CategoriaVeiculo != null
                        ? v.CategoriaVeiculo.Nome
                        : null
                })
                .ToListAsync();

            return Ok(veiculos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var veiculo = await _context.Veiculos
                .Include(v => v.Fabricante)
                .Include(v => v.CategoriaVeiculo)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veiculo == null)
                return NotFound(new { mensagem = "Veículo não encontrado." });

            return Ok(veiculo);
        }

        [HttpPost]
        public async Task<ActionResult<Veiculo>> Post(Veiculo veiculo)
        {
            var fabricanteExiste =
                await _context.Fabricantes.AnyAsync(
                    f => f.Id == veiculo.FabricanteId);

            if (!fabricanteExiste)
                return BadRequest(new
                {
                    mensagem = "Fabricante informado não existe."
                });

            var categoriaExiste =
                await _context.CategoriasVeiculos.AnyAsync(
                    c => c.Id == veiculo.CategoriaVeiculoId);

            if (!categoriaExiste)
                return BadRequest(new
                {
                    mensagem = "Categoria informada não existe."
                });

            try
            {
                _context.Veiculos.Add(veiculo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(Get),
                    new { id = veiculo.Id },
                    veiculo);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    mensagem = "Não foi possível cadastrar. Verifique a placa."
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Veiculo veiculo)
        {
            var existente = await _context.Veiculos.FindAsync(id);

            if (existente == null)
                return NotFound(new { mensagem = "Veículo não encontrado." });

            existente.Placa = veiculo.Placa;
            existente.Modelo = veiculo.Modelo;
            existente.AnoFabricacao = veiculo.AnoFabricacao;
            existente.Quilometragem = veiculo.Quilometragem;
            existente.Disponivel = veiculo.Disponivel;
            existente.FabricanteId = veiculo.FabricanteId;
            existente.CategoriaVeiculoId = veiculo.CategoriaVeiculoId;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    mensagem = "Erro ao atualizar o veículo."
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);

            if (veiculo == null)
                return NotFound(new { mensagem = "Veículo não encontrado." });

            try
            {
                _context.Veiculos.Remove(veiculo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    mensagem = "O veículo possui aluguéis vinculados."
                });
            }
        }
    }
}