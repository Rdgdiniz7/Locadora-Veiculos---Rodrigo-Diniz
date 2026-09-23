using LocadoraVeiculos.Data;
using LocadoraVeiculos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocadoraVeiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlugueisController : ControllerBase
    {
        private readonly LocadoraContext _context;

        public AlugueisController(LocadoraContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var alugueis = await _context.Alugueis
                .Include(a => a.Cliente)
                .Include(a => a.Veiculo)
                .Select(a => new
                {
                    a.Id,

                    Cliente = a.Cliente != null
                        ? a.Cliente.Nome
                        : null,

                    Veiculo = a.Veiculo != null
                        ? a.Veiculo.Modelo
                        : null,

                    a.DataInicio,
                    a.DataFimPrevista,
                    a.DataDevolucao,
                    a.QuilometragemInicial,
                    a.QuilometragemFinal,
                    a.ValorDiaria,
                    a.ValorTotal,
                    a.Status
                })
                .ToListAsync();

            return Ok(alugueis);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var aluguel = await _context.Alugueis
                .Include(a => a.Cliente)
                .Include(a => a.Veiculo)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluguel == null)
                return NotFound(new { mensagem = "Aluguel não encontrado." });

            return Ok(aluguel);
        }

        [HttpPost]
        public async Task<ActionResult<Aluguel>> Post(Aluguel aluguel)
        {
            var clienteExiste =
                await _context.Clientes.AnyAsync(
                    c => c.Id == aluguel.ClienteId);

            if (!clienteExiste)
                return BadRequest(new
                {
                    mensagem = "Cliente informado não existe."
                });

            var veiculo =
                await _context.Veiculos.FindAsync(aluguel.VeiculoId);

            if (veiculo == null)
                return BadRequest(new
                {
                    mensagem = "Veículo informado não existe."
                });

            if (!veiculo.Disponivel)
                return BadRequest(new
                {
                    mensagem = "O veículo não está disponível."
                });

            if (aluguel.DataFimPrevista <= aluguel.DataInicio)
                return BadRequest(new
                {
                    mensagem = "A data final deve ser posterior à data inicial."
                });

            aluguel.QuilometragemInicial = veiculo.Quilometragem;
            aluguel.Status = "Ativo";

            veiculo.Disponivel = false;

            _context.Alugueis.Add(aluguel);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = aluguel.Id },
                aluguel);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Aluguel aluguel)
        {
            var existente = await _context.Alugueis.FindAsync(id);

            if (existente == null)
                return NotFound(new { mensagem = "Aluguel não encontrado." });

            existente.DataInicio = aluguel.DataInicio;
            existente.DataFimPrevista = aluguel.DataFimPrevista;
            existente.DataDevolucao = aluguel.DataDevolucao;
            existente.QuilometragemInicial = aluguel.QuilometragemInicial;
            existente.QuilometragemFinal = aluguel.QuilometragemFinal;
            existente.ValorDiaria = aluguel.ValorDiaria;
            existente.ValorTotal = aluguel.ValorTotal;
            existente.Status = aluguel.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/devolver")]
        public async Task<IActionResult> Devolver(
            int id,
            int quilometragemFinal)
        {
            var aluguel = await _context.Alugueis
                .Include(a => a.Veiculo)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluguel == null)
                return NotFound(new
                {
                    mensagem = "Aluguel não encontrado."
                });

            if (quilometragemFinal < aluguel.QuilometragemInicial)
                return BadRequest(new
                {
                    mensagem = "Quilometragem final inválida."
                });

            aluguel.QuilometragemFinal = quilometragemFinal;
            aluguel.DataDevolucao = DateTime.Now;
            aluguel.Status = "Finalizado";

            if (aluguel.Veiculo != null)
            {
                aluguel.Veiculo.Quilometragem = quilometragemFinal;
                aluguel.Veiculo.Disponivel = true;
            }

            var dias = Math.Max(
                1,
                (int)Math.Ceiling(
                    (aluguel.DataDevolucao.Value - aluguel.DataInicio)
                    .TotalDays));

            aluguel.ValorTotal = dias * aluguel.ValorDiaria;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Veículo devolvido com sucesso.",
                aluguel.Id,
                aluguel.ValorTotal
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var aluguel = await _context.Alugueis.FindAsync(id);

            if (aluguel == null)
                return NotFound(new { mensagem = "Aluguel não encontrado." });

            _context.Alugueis.Remove(aluguel);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}