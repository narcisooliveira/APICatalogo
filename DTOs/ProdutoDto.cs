namespace APICatalogo.DTOs
{
    public class ProdutoDto
    {
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public string? ImagemUrl { get; set; }
        public int CategoriaId { get; set; }
        public CategoriaDto? Categoria { get; set; }
    }
}
