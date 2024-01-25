using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {

        public ProdutoRepository(ApiCatalogoContext contexto) : base (contexto)
        { }

        public IEnumerable<Produto?> GetProdutosByPreco()
        {
            return Get().OrderBy(p => p.Preco).ToList();
        }

        public PagedList<Produto?> GetProdutos(ProdutosParameters produtosParameters)
        {
            return PagedList<Produto?>.ToPagedList(Get().OrderBy(on => on.ProdutoId),
                               produtosParameters.PageNumber,
                                              produtosParameters.PageSize);
        }
    }
}
