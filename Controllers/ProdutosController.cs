using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Models;
using APICatalogo.Filters;
using APICatalogo.Repository;

namespace APICatalogo.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProdutoRepositoryController : Controller
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;

        public ProdutoRepositoryController(IUnitOfWork unitOfWork, ILogger<ProdutoRepositoryController> logger)
        {
            _uof = unitOfWork;
            _logger = logger;
        }

        // GET: ProdutoRepository
        [HttpGet("Produtos")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<List<Produto>> Index()
        {
            // Melhorando a performance com AsNoTracking() e restrinjindo a quantidade de registros com Take(10)
            var produtos = _uof.ProdutoRepository?.Get()
                                                 .AsNoTracking()
                                                 .Take(10)
                                                 .ToList();

            if (produtos == null)
            {
                return NotFound("Produtos não encontrado!");
            }

            return Ok(produtos);
        }

        //GET: ProdutoRepository/Details
        [HttpGet("produto/{id}")]
        public async Task<ActionResult<Produto>> Details(int? id)
        {
            _logger.LogInformation($"GET api/produto/{id} foi solicitado");

            if (id == null || _uof.ProdutoRepository == null)
            {
                return NotFound("Produto não encontrado!");
            }

            // Melhorando a performance com AsNoTracking()
            var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        // POST: ProdutoRepository/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("produto")]
        public ActionResult Create(Produto produto)
        {

            if (ModelState.IsValid)
            {
                _uof.ProdutoRepository.Add(produto);
                _uof.Commit();
                
                return Ok(produto);
            }

            return BadRequest();
        }

        // PUT: ProdutoRepository/Edit/{id}
        [HttpPut("produto/{id}")]
        public ActionResult Edit(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
                return NotFound("Produto não encontrado!");

            if (ModelState.IsValid)
            {
                try
                {
                    _uof.ProdutoRepository.Update(produto);
                    _uof.Commit();
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

        // DELETE: ProdutoRepository/Delete/{id}
        [HttpDelete("produto/{id}")]
        public ActionResult Delete(int? id)
        {
            if (id == null || _uof.ProdutoRepository == null)
            {
                return NotFound("Produto não encontrado!");
            }

            var produto = _uof.ProdutoRepository.GetById(m => m.ProdutoId == id);

            if (produto == null)
            {
                return NotFound("Produto não encontrado!");
            }

            _uof.ProdutoRepository.Delete(produto);
            _uof.Commit();

            return Ok(produto);
        }

        private bool ProdutoExists(int id)
        {
            return (_uof.ProdutoRepository.Get().Any(e => e.ProdutoId == id));
        }
    }
}
