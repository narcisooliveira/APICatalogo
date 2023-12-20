using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    public class Produto
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int ProdutoId { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [Column(TypeName = "varchar(80)")]
        public string? Nome { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [Column(TypeName = "varchar(500)")]
        public string? Descricao { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [Column(TypeName = "varchar(500)")]
        public string? ImagemUrl { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; } // Navigation Property
    }
}
