using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FabricantesController : ControllerBase
    {
        private readonly LocadoraContext _context;

        public FabricantesController(LocadoraContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fabricante>>> Get()
        {
            return await _context.Fabricantes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Fabricante>> Get(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);

            if (fabricante == null)
                return NotFound(new { mensagem = "Fabricante não encontrado." });

            return fabricante;
        }

        [HttpPost]
        public async Task<ActionResult<Fabricante>> Post(Fabricante fabricante)
        {
            try
            {
                _context.Fabricantes.Add(fabricante);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(Get),
                    new { id = fabricante.Id },
                    fabricante);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    mensagem = "Não foi possível cadastrar o fabricante. Verifique se ele já existe."
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Fabricante fabricante)
        {
            var existente = await _context.Fabricantes.FindAsync(id);

            if (existente == null)
                return NotFound(new { mensagem = "Fabricante não encontrado." });

            existente.Nome = fabricante.Nome;
            existente.PaisOrigem = fabricante.PaisOrigem;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var fabricante = await _context.Fabricantes.FindAsync(id);

            if (fabricante == null)
                return NotFound(new { mensagem = "Fabricante não encontrado." });

            try
            {
                _context.Fabricantes.Remove(fabricante);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    mensagem = "O fabricante possui veículos vinculados e não pode ser excluído."
                });
            }
        }
    }
}