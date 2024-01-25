using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        IEnumerable<Categoria?> GetCategoriasProdutos();
        PagedList<Categoria?> GetCategorias(CategoriaParameters categoriaParameters);
        PagedList<Categoria?> GetCategoriasProdutos(CategoriaParameters categoriaParameters);
    }
}
