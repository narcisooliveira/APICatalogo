using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<IEnumerable<Produto?>> GetProdutosByPreco();
        Task<PagedList<Produto?>> GetProdutos(ProdutosParameters produtosParameters);
    }
}
