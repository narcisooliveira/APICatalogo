using APICatalogo.Context;

namespace APICatalogo.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ProdutoRepository _produtoRepo;
        private CategoriaRepository _categoriaRepo;
        private readonly ApiCatalogoContext _context;

        public UnitOfWork(ApiCatalogoContext contexto)
        {
            _context = contexto;
        }

        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepo ??= new ProdutoRepository(_context);
            }
        }

        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                return _categoriaRepo ??= new CategoriaRepository(_context);
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        // Libera os recursos da memória que estão sendo utilizados pelo contexto
        public void Dispose()
        {
               _context.Dispose();
        }
    }
}
