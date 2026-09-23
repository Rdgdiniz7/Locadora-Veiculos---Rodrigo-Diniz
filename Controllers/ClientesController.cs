using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly LocadoraContext _context;

        public ClientesController(LocadoraContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> Get()
        {
            return await _context.Clientes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> Get(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound(new { mensagem = "Cliente não encontrado." });

            return cliente;
        }

        [HttpPost]
        public async Task<ActionResult<Cliente>> Post(Cliente cliente)
        {
            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(Get),
                    new { id = cliente.Id },
                    cliente);
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    mensagem = "CPF ou e-mail já cadastrado."
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Cliente cliente)
        {
            var existente = await _context.Clientes.FindAsync(id);

            if (existente == null)
                return NotFound(new { mensagem = "Cliente não encontrado." });

            existente.Nome = cliente.Nome;
            existente.CPF = cliente.CPF;
            existente.Email = cliente.Email;
            existente.Telefone = cliente.Telefone;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    mensagem = "CPF ou e-mail já utilizado."
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound(new { mensagem = "Cliente não encontrado." });

            try
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    mensagem = "O cliente possui aluguéis e não pode ser excluído."
                });
            }
        }
    }
}