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

        public async Task<PagedList<Categoria?>> GetCategorias(CategoriaParameters categoriaParameters)
        {
            return await PagedList<Categoria?>.ToPagedList(Get().OrderBy(on => on.CategoriaId),
                                              categoriaParameters.PageNumber,
                                              categoriaParameters.PageSize);           
        }

        public async Task<IEnumerable<Categoria?>> GetCategoriasProdutos()
        {
            return await Get().Include(x => x.Produtos).ToListAsync();
        }

        public async Task<PagedList<Categoria?>> GetCategoriasProdutos(CategoriaParameters categoriaParameters)
        {
            return await PagedList<Categoria?>.ToPagedList(Get().Include(x => x.Produtos)
                                                                .OrderBy(on => on.CategoriaId),
                                                                categoriaParameters.PageNumber,
                                                                categoriaParameters.PageSize);     
            
        }
    }
  
}
