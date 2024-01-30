using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {

        public ProdutoRepository(ApiCatalogoContext contexto) : base (contexto)
        { }

        public async Task<IEnumerable<Produto?>> GetProdutosByPreco()
        {
            return await Get().OrderBy(p => p.Preco).ToListAsync();
        }

        public async Task<PagedList<Produto?>> GetProdutos(ProdutosParameters produtosParameters)
        {
            return await PagedList<Produto?>.ToPagedList(Get().OrderBy(on => on.ProdutoId),
                                                                    produtosParameters.PageNumber,
                                                                    produtosParameters.PageSize);
        }
    }
}
