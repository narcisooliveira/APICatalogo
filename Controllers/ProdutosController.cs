using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Models;
using APICatalogo.Filters;
using APICatalogo.Repository;
using AutoMapper;
using APICatalogo.DTOs;
using APICatalogo.Pagination;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProdutosController : Controller
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork unitOfWork, ILogger<ProdutosController> logger, IMapper mapper)
        {
            _uof = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: ProdutoRepository
        [HttpGet("Produtos")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll([FromQuery] ProdutosParameters produtosParameters)
        {
            _logger.LogInformation($"GET api/produtos foi solicitado");

            // Melhorando a performance com AsNoTracking() e restrinjindo a quantidade de registros com Take(10)
            var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters);

            if (produtos == null)
                return NotFound("Produtos não encontrado!");

            _logger.LogInformation($"GET api/produtos retornou {produtos.Count} produtos");

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(produtos.MetaData));

            var produtosDto = _mapper.Map<List<ProdutoDto>>(produtos);

            return Ok(produtosDto);
        }

        //GET: ProdutoRepository/Details
        [HttpGet("produto/{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<Produto>> GetById(int? id)
        {
            _logger.LogInformation($"GET api/produto/{id} foi solicitado");

            if (id == null)
                return NotFound("Produto não encontrado!");

            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
                return NotFound("Produto não encontrado!");         

            _logger.LogInformation($"GET api/produto/{id} retornou o produto {produto.Nome}");

            var produtoDto = _mapper.Map<ProdutoDto>(produto);

            return Ok(produtoDto);
        }

        // POST: ProdutoRepository/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("produto")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Create(ProdutoDto produtoDto)
        {
            _logger.LogInformation($"POST api/produto foi solicitado");

            if (produtoDto == null)
                return NotFound("Produto não informado!");

            var produto = _mapper.Map<Produto>(produtoDto);

            if (ModelState.IsValid)
            {
                _uof.ProdutoRepository.Add(produto);
                await _uof.Commit();

                _logger.LogInformation($"POST api/produto foi criado com sucesso");

                return Ok(produtoDto);
            }

            return BadRequest();
        }

        // PUT: ProdutoRepository/Edit/{id}
        [HttpPut("produto/{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Edit(int id, [FromBody] ProdutoDto produtoDto)
        {
            if (id != produtoDto.ProdutoId)
                return NotFound("Produto não informado!");

            var produto = _mapper.Map<Produto>(produtoDto);

            if (ModelState.IsValid)
            {
                try
                {
                    _uof.ProdutoRepository.Update(produto);
                    await _uof.Commit();

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
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound("Produto não encontrado!");

            var produto = await _uof.ProdutoRepository.GetById(m => m.ProdutoId == id);

            if (produto == null)
                return NotFound("Produto não encontrado!");

            _uof.ProdutoRepository.Delete(produto);
            await _uof.Commit();

            _logger.LogInformation($"DELETE api/produto foi deletado com sucesso");

            var produtoDto = _mapper.Map<ProdutoDto>(produto);

            return Ok(produtoDto);
        }

        private bool Exists(int id)
        {
            return _uof.ProdutoRepository.Get().Any(e => e.ProdutoId == id);
        }
    }
}
