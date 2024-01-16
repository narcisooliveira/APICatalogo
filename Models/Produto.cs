using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Models
{
    public class Produto
    {
        [Key]
        public int ProdutoId { get; set; }

        [Required]
        [StringLength(80, ErrorMessage = "O nome deve ter entre 5 e 80 caracteres", MinimumLength = 5)]
        [Column(TypeName = "varchar(80)")]
        public string? Nome { get; set; }

        [Required]
        [StringLength(300, ErrorMessage = "A descrição deve ter entre 5 e 300 caracteres", MinimumLength = 5)]
        [Column(TypeName = "varchar(500)")]
        public string? Descricao { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "O preço deve estar entre 1 e 10000 reais")]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "A descrição deve ter entre 5 e 500 caracteres", MinimumLength = 5)]
        [Column(TypeName = "varchar(500)")]
        public string? ImagemUrl { get; set; }
        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }
        public int CategoriaId { get; set; }

        [JsonIgnore]
        public Categoria? Categoria { get; set; } // Navigation Property
    }
}
