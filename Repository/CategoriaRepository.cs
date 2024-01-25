using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ApiCatalogoContext contexto) : base(contexto)
        {
        }

        public PagedList<Categoria?> GetCategorias(CategoriaParameters categoriaParameters)
        {
            return PagedList<Categoria?>.ToPagedList(Get().OrderBy(on => on.CategoriaId),
                                              categoriaParameters.PageNumber,
                                              categoriaParameters.PageSize);           
        }

        public IEnumerable<Categoria?> GetCategoriasProdutos()
        {
            return Get().Include(x => x.Produtos);
        }

        public PagedList<Categoria?> GetCategoriasProdutos(CategoriaParameters categoriaParameters)
        {
            return PagedList<Categoria?>.ToPagedList(Get().Include(x => x.Produtos)
                                                            .OrderBy(on => on.CategoriaId),
                                                            categoriaParameters.PageNumber,
                                                            categoriaParameters.PageSize);
            
        }
    }
  
}
