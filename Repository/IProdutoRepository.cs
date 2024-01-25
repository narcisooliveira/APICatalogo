using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        IEnumerable<Produto?> GetProdutosByPreco();
        PagedList<Produto?> GetProdutos(ProdutosParameters produtosParameters);
    }
}
