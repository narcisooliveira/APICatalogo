using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repository;
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
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;

        public CategoriasController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _uof = unitOfWork;
            _configuration = configuration;
        }

        [HttpGet("autor")]
        public string GetAutor()
        {
            var autor = _configuration["autor"];
            var conexao = _configuration["ConnectionStrings:DefaultConnection"];
            return $"Autor: {autor} - Conexão: {conexao}";
        }

        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> GetSaudacao([FromServices] IMeuServico meuServico, [BindRequired] string nome)
        {
            return meuServico.Saudacao(nome);
        }

        // GET: Categorias
        [HttpGet("categorias")]
        [HttpGet("teste")]
        public ActionResult<IEnumerable<Categoria>> Index()
        {
            // Melhorando a performance com AsNoTracking() e restrinjindo a quantidade de registros com Take(10)
            var apiCatalogoContext = _uof.CategoriaRepository.Get().ToList();

            if (apiCatalogoContext == null)
            {
                return NotFound("Categorias não encontradas!");
            }

            return Ok(apiCatalogoContext);
        }

        // GET: Categorias/Details
        [HttpGet("categoria/{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<Categoria> Details(int? id, [BindRequired] string? name)
        {
            if (id == null || string.IsNullOrEmpty(name))
            {
                return NotFound("Categoria não encontrada!");
            }

            var Categoria = _uof.CategoriaRepository.GetById(m => m.CategoriaId == id || m.Nome.Equals(name));

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
            var produtos = _uof.CategoriaRepository.GetCategoriasProdutos().ToList();

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
        public ActionResult Create(Categoria Categoria)
        {

            if (ModelState.IsValid)
            {
                _uof.CategoriaRepository.Add(Categoria);
                _uof.Commit();
                return Ok(Categoria);
            }

            return BadRequest();
        }

        // PUT: Categorias/Edit/{id}
        [HttpPut("categoria/{id}")]
        public  ActionResult Edit(int id, Categoria Categoria)
        {
            if (id != Categoria.CategoriaId)
                return NotFound("Categoria não encontrada!");

            if (ModelState.IsValid)
            {
                try
                {
                    _uof.CategoriaRepository.Update(Categoria);
                    _uof.Commit();
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
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            var Categoria = _uof.CategoriaRepository.GetById(m => m.CategoriaId == id);

            if (Categoria == null)
            {
                return NotFound("Categoria não encontrada!");
            }

            _uof.CategoriaRepository.Delete(Categoria);
            _uof.Commit();

            return Ok(Categoria);
        }

        private bool CategoriaExists(int id)
        {
            return id.Equals(_uof.CategoriaRepository.GetById(e => e.CategoriaId == id));
        }
    }
}
