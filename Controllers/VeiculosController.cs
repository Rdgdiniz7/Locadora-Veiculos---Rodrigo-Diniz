using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasVeiculosController : ControllerBase
    {
        private readonly LocadoraContext _context;

        public CategoriasVeiculosController(LocadoraContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaVeiculo>>> Get()
        {
            return await _context.CategoriasVeiculos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaVeiculo>> Get(int id)
        {
            var categoria = await _context.CategoriasVeiculos.FindAsync(id);

            if (categoria == null)
                return NotFound(new { mensagem = "Categoria não encontrada." });

            return categoria;
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaVeiculo>> Post(
            CategoriaVeiculo categoria)
        {
            try
            {
                _context.CategoriasVeiculos.Add(categoria);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(Get),
                    new { id = categoria.Id },
                    categoria);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    mensagem = "Não foi possível cadastrar a categoria."
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(
            int id,
            CategoriaVeiculo categoria)
        {
            var existente =
                await _context.CategoriasVeiculos.FindAsync(id);

            if (existente == null)
                return NotFound(new { mensagem = "Categoria não encontrada." });

            existente.Nome = categoria.Nome;
            existente.Descricao = categoria.Descricao;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria =
                await _context.CategoriasVeiculos.FindAsync(id);

            if (categoria == null)
                return NotFound(new { mensagem = "Categoria não encontrada." });

            try
            {
                _context.CategoriasVeiculos.Remove(categoria);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    mensagem = "A categoria possui veículos vinculados."
                });
            }
        }
    }
}