using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace APICatalogo.Context
{
    public class ApiCatalogoContext : IdentityDbContext<ApplicationUser>
    {
        public ApiCatalogoContext(DbContextOptions<ApiCatalogoContext> options) : base(options)
        {
        }

        public DbSet<Categoria>? Categorias { get; set; }
        public DbSet<Produto>? Produtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
