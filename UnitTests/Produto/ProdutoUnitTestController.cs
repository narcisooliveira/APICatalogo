using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UnitTests.Produto;

public class ProdutoUnitTestController
{
    public IUnitOfWork repository;
    public IMapper mapper;
    public ILogger<ProdutosController> logger;
    public static DbContextOptions<ApiCatalogoContext> dbContextOptions;
    public static string ConnectionString =>
        "Server=localhost;Database=CatalogoDB;Uid=narciso;Pwd=p@ssWord;";

    static ProdutoUnitTestController()
    {
        dbContextOptions = new DbContextOptionsBuilder<ApiCatalogoContext>()
            .UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString))
            .Options;
    }

    public ProdutoUnitTestController()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });

        mapper = config.CreateMapper();

        var context = new ApiCatalogoContext(dbContextOptions);

        repository = new UnitOfWork(context);

        var factory = LoggerFactory.Create(builder => builder.AddConsole());

        logger = factory.CreateLogger<ProdutosController>();
    }
}