using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : Controller
    {
        private readonly ApiCatalogoContext _context;

        public ProdutosController(ApiCatalogoContext context)
        {
            _context = context;
        }

        // GET: Produtos
        [HttpGet]
        public ActionResult<List<Produto>> Index()
        {
            // Melhorando a performance com AsNoTracking() e restrinjindo a quantidade de registros com Take(10)
            var apiCatalogoContext = _context.Produtos?.AsNoTracking().Take(10).ToList();

            if (apiCatalogoContext == null)
            {
                return NotFound("Produtos não encontrado!");
            }

            return Ok(apiCatalogoContext.ToList());
        }

        //GET: Produtos/Details
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Produtos == null)
            {
                return NotFound("Produto não encontrado!");
            }

            // Melhorando a performance com AsNoTracking()
            var produto = await _context.Produtos.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        // POST: Produtos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(Produto produto)
        {

            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                
                return Ok(produto);
            }

            return BadRequest();
        }

        // PUT: Produtos/Edit/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, Produto produto)
        {
            if (id != produto.ProdutoId || _context.Produtos == null)
                return NotFound("Produto não encontrado!");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ProdutoExists(produto.ProdutoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception($"Erro ao atualizar produto {ex.Message}");
                    }
                }

                return Ok(produto);
            }

            return BadRequest();
        }

        // DELETE: Produtos/Delete/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Produtos == null)
            {
                return NotFound("Produto não encontrado!");
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.ProdutoId == id);

            if (produto == null)
            {
                return NotFound("Produto não encontrado!");
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return Ok(produto);
        }

        private bool ProdutoExists(int id)
        {
            return (_context.Produtos?.Any(e => e.ProdutoId == id)).GetValueOrDefault();
        }
    }
}
