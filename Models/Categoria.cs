using System.Collections.ObjectModel;

namespace APICatalogo.Models
{
    public class Categoria
    {
        public Categoria()
        {
            Produtos = new Collection<Produto>();
        }

        [System.ComponentModel.DataAnnotations.Key]
        public int CategoriaId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(80)]
        public string? Nome { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(500)]
        public string? ImagemUrl { get; set; }
        public ICollection<Produto>? Produtos { get; set; }

    }
}
