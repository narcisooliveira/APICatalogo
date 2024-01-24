using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Models;
using APICatalogo.Filters;
using APICatalogo.Repository;
using AutoMapper;
using APICatalogo.DTOs;

namespace APICatalogo.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProdutoRepositoryController : Controller
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ProdutoRepositoryController(IUnitOfWork unitOfWork, ILogger<ProdutoRepositoryController> logger, IMapper mapper)
        {
            _uof = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: ProdutoRepository
        [HttpGet("Produtos")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<ProdutoDTO>> GetAll()
        {
            _logger.LogInformation($"GET api/produtos foi solicitado");

            // Melhorando a performance com AsNoTracking() e restrinjindo a quantidade de registros com Take(10)
            var produtos = _uof.ProdutoRepository?.Get()
                                                 .AsNoTracking()
                                                 .Take(10)
                                                 .ToList();

            if (produtos == null)
                return NotFound("Produtos não encontrado!");

            _logger.LogInformation($"GET api/produtos retornou {produtos.Count} produtos");

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        //GET: ProdutoRepository/Details
        [HttpGet("produto/{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<Produto> GetById(int? id)
        {
            _logger.LogInformation($"GET api/produto/{id} foi solicitado");

            if (id == null)
                return NotFound("Produto não encontrado!");

            var produto = _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
                return NotFound("Produto não encontrado!");         

            _logger.LogInformation($"GET api/produto/{id} retornou o produto {produto.Nome}");

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }

        // POST: ProdutoRepository/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("produto")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult Create(ProdutoDTO produtoDto)
        {
            _logger.LogInformation($"POST api/produto foi solicitado");

            if (produtoDto == null)
                return NotFound("Produto não informado!");

            var produto = _mapper.Map<Produto>(produtoDto);

            if (ModelState.IsValid)
            {
                _uof.ProdutoRepository.Add(produto);
                _uof.Commit();

                _logger.LogInformation($"POST api/produto foi criado com sucesso");

                return Ok(produtoDto);
            }

            return BadRequest();
        }

        // PUT: ProdutoRepository/Edit/{id}
        [HttpPut("produto/{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult Edit(int id, [FromBody] ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
                return NotFound("Produto não informado!");

            var produto = _mapper.Map<Produto>(produtoDto);

            if (ModelState.IsValid)
            {
                try
                {
                    _uof.ProdutoRepository.Update(produto);
                    _uof.Commit();

                    _logger.LogInformation($"PUT api/produto foi atualizado com sucesso");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!Exists(produto.ProdutoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception($"Erro ao atualizar produto {ex.Message}");
                    }
                }

                return Ok(produtoDto);
            }

            return BadRequest();
        }

        // DELETE: ProdutoRepository/Delete/{id}
        [HttpDelete("produto/{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound("Produto não encontrado!");

            var produto = _uof.ProdutoRepository.GetById(m => m.ProdutoId == id);

            if (produto == null)
                return NotFound("Produto não encontrado!");

            _uof.ProdutoRepository.Delete(produto);
            _uof.Commit();

            _logger.LogInformation($"DELETE api/produto foi deletado com sucesso");

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }

        private bool Exists(int id)
        {
            return (_uof.ProdutoRepository.Get().Any(e => e.ProdutoId == id));
        }
    }
}
