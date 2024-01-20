using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ApiCatalogoContext contexto) : base(contexto)
        {
        }

        public IEnumerable<Categoria?> GetCategoriasProdutos()
        {
            return Get().Include(x => x.Produtos);
        }
    }
  
}
