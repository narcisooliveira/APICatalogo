using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategoriasController : Controller
    {
        private readonly ApiCatalogoContext _context;

        public CategoriasController(ApiCatalogoContext context)
        {
            _context = context;
        }

        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuServico, [BindRequired] string nome)
        {
            return meuServico.Saudacao(nome);
        }

        // GET: Categorias
        [HttpGet("categorias")]
        [HttpGet("teste")]
        public ActionResult<List<Categoria>> Index()
        {
            // Melhorando a performance com AsNoTracking() e restrinjindo a quantidade de registros com Take(10)
            var apiCatalogoContext = _context.Categorias?.AsNoTracking().Take(10).ToList();

            if (apiCatalogoContext == null)
            {
                return NotFound("Categorias não encontradas!");
            }

            return Ok(apiCatalogoContext.ToList());
        }

        // GET: Categorias/Details
        [HttpGet("categoria/{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<Categoria>> Details(int? id, [BindRequired] string? name)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            var Categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.CategoriaId == id || m.Nome.Equals(name));
            if (Categoria == null)
            {
                return NotFound();
            }

            return Ok(Categoria);
        }

        // GET: Produtos/Categorias
        [HttpGet("produtos/categorias")]
        public ActionResult<IEnumerable<Produto>>  Produtos()
        {
            // Melhorando a performance com AsNoTracking(), usando filtro Where para trazer apenas os 5 primeiros Ids de Categoria e restrinjindo a quantidade de registros com Take(10) para melhorar a performance.
            var produtos = _context.Produtos?
                .Include(x => x.Categoria).Where(x => x.CategoriaId <= 5).AsNoTracking().Take(10).ToList();

            if (produtos == null)
            {
                return NotFound("Produtos não encontrados.");
            }

            return Ok(produtos);
        }
        
        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("categoria")]
        public async Task<IActionResult> Create(Categoria Categoria)
        {

            if (ModelState.IsValid)
            {
                _context.Add(Categoria);
                await _context.SaveChangesAsync();

                return Ok(Categoria);
            }

            return BadRequest();
        }

        // PUT: Categorias/Edit/{id}
        [HttpPut("categoria/{id}")]
        public async Task<IActionResult> Edit(int id, Categoria Categoria)
        {
            if (id != Categoria.CategoriaId || _context.Categorias == null)
                return NotFound("Categoria não encontrada!");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CategoriaExists(Categoria.CategoriaId))
                    {
                        return NotFound("Categoria não encontrada.");
                    }
                    else
                    {
                        throw new Exception($"Erro ao atualizar Categoria {ex.Message}");
                    }
                }

                return Ok(Categoria);
            }

            return BadRequest();
        }

        // DELETE: Categorias/Delete/{id}
        [HttpDelete("categoria/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categorias == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            var Categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.CategoriaId == id);

            if (Categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            _context.Categorias.Remove(Categoria);
            await _context.SaveChangesAsync();

            return Ok(Categoria);
        }

        private bool CategoriaExists(int id)
        {
            return (_context.Categorias?.Any(e => e.CategoriaId == id)).GetValueOrDefault();
        }
    }
}
