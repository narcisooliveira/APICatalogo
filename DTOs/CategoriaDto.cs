namespace APICatalogo.DTOs
{
    public class CategoriaDto
    {
        public int CategoriaId { get; set; }
        public string? Nome { get; set; }
        public string? ImagemUrl { get; set; }
        
        public ICollection<ProdutoDto>? Produtos { get; set; }
    }
}
