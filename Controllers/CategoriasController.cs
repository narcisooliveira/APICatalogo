using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Route("api")]
    [ApiController]
    public class CategoriasController : Controller
    {
        private readonly IUnitOfWork _uof;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<CategoriasController> logger, IMapper mapper)
        {
            _uof = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
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
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAll([FromQuery] CategoriaParameters categoriaParameters)
        {
            _logger.LogInformation($"GET api/categorias foi solicitado");

            // Melhorando a performance com AsNoTracking() e restrinjindo a quantidade de registros com Take(10)
            var categorias = await _uof.CategoriaRepository.GetCategorias(categoriaParameters);

            if (categorias == null)
            {
                _logger.LogError($"GET api/categorias não retornou categorias");
                return NotFound("Categorias não encontradas!");
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(categorias.MetaData));

            var categoriaDTOs = _mapper.Map<List<CategoriaDTO>>(categorias);

            _logger.LogInformation($"GET api/categorias retornou {categorias.Count} categorias");

            return Ok(categoriaDTOs);
        }

        // GET: Categorias/Details
        [HttpGet("categoria/{id:int:min(1)}", Name = "ObterProduto")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<CategoriaDTO>> Details(int? id, [BindRequired] string? name)
        {
            _logger.LogInformation($"GET api/categoria/{id} foi solicitado");

            if (id == null || string.IsNullOrEmpty(name))
            {
                _logger.LogError($"GET api/categoria/{id} não retornou categorias");
                return NotFound("Categoria não encontrada!");
            }

            var Categoria = await _uof.CategoriaRepository.GetById(m => m.CategoriaId == id || m.Nome.Equals(name));

            if (Categoria == null)
            {
                _logger.LogError($"GET api/categoria/{id} não retornou categorias");
                return NotFound("Categoria não encontrada!");
            }

            _logger.LogInformation($"GET api/categoria/{id} retornou a categoria {Categoria.Nome}");

            var categoriaDTO = _mapper.Map<CategoriaDTO>(Categoria);

            return Ok(categoriaDTO);
        }

        // GET: Produtos/Categorias
        [HttpGet("produtos/categorias")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Produtos([FromQuery] CategoriaParameters categoriaParameters)
        {
            _logger.LogInformation($"GET api/produtos/categorias foi solicitado");

            // Melhorando a performance com AsNoTracking(), usando filtro Where para trazer apenas os 5 primeiros Ids de Categoria e restrinjindo a quantidade de registros com Take(10) para melhorar a performance.
            var produtos = await _uof.CategoriaRepository.GetCategoriasProdutos(categoriaParameters);

            if (produtos == null)
            {
                _logger.LogError($"GET api/produtos/categorias não retornou produtos");
                return NotFound("Produtos não encontrados.");
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(produtos.MetaData));

            _logger.LogInformation($"GET api/produtos/categorias retornou {produtos.Count} produtos");

            var produtosDTOs = _mapper.Map<List<ProdutoDTO>>(produtos);

            return Ok(produtosDTOs);
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("categoria")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult Create(CategoriaDTO CategoriaDto)
        {
            _logger.LogInformation($"POST api/categoria foi solicitado");

            if (ModelState.IsValid)
            {
                var categoria = _mapper.Map<Categoria>(CategoriaDto);

                _uof.CategoriaRepository.Add(categoria);
                _uof.Commit();

                _logger.LogInformation($"POST api/categoria foi criado com sucesso");

                return Ok(categoria);
            }

            _logger.LogInformation($"POST api/categoria não foi criado");

            return BadRequest();
        }

        // PUT: Categorias/Edit/{id}
        [HttpPut("categoria/{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult Edit(int id, CategoriaDTO categoriaDto)
        {
            _logger.LogInformation($"PUT api/categoria/{id} foi solicitado");

            if (id != categoriaDto.CategoriaId)
                return NotFound("Categoria não encontrada!");

            if (ModelState.IsValid)
            {
                var categoria = _mapper.Map<Categoria>(categoriaDto);

                try
                {
                    _uof.CategoriaRepository.Update(categoria);
                    _uof.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!Exists(categoria.CategoriaId))
                    {
                        _logger.LogError($"PUT api/categoria/{id} Categoria não encontrada. {ex.Message}");
                        return NotFound("Categoria não encontrada.");
                    }
                    else
                    {
                        _logger.LogError($"PUT api/categoria/{id} Erro ao atualizar Categoria {ex.Message}");
                        throw new Exception($"Erro ao atualizar Categoria {ex.Message}");
                    }
                }

                _logger.LogInformation($"PUT api/categoria/{id} foi atualizado com sucesso");

                return Ok(categoriaDto);
            }

            _logger.LogInformation($"PUT api/categoria/{id} não foi atualizado");

            return BadRequest();
        }

        // DELETE: Categorias/Delete/{id}
        [HttpDelete("categoria/{id}")]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult> Delete(int? id)
        {
            _logger.LogInformation($"DELETE api/categoria/{id} foi solicitado");

            if (id == null)
            {
                _logger.LogError($"DELETE api/categoria/{id} Categoria não encontrada");
                return NotFound("Categoria não encontrada!");
            }

            var categoria = await _uof.CategoriaRepository.GetById(m => m.CategoriaId == id);

            if (categoria == null)
            {
                _logger.LogError($"DELETE api/categoria/{id} Categoria não encontrada");
                return NotFound("Categoria não encontrada!");
            }

            _uof.CategoriaRepository.Delete(categoria);
            await _uof.Commit();

            _logger.LogInformation($"DELETE api/categoria/{id} foi deletado com sucesso");

            var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDto);
        }

        private bool Exists(int id)
        {
            return id.Equals(_uof.CategoriaRepository.GetById(e => e.CategoriaId == id));
        }
    }
}
