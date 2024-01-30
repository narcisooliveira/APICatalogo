using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    public partial class PopulaProdutos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) VALUES('Coca-Cola Diet', 'Refrigerante de Cola 350ml', 5.45, 'coca-cola.jpg', 50, now(), (SELECT CategoriaId FROM Categorias WHERE Nome='Bebidas'))");
            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) VALUES('Pizza Calabresa', 'Pizza de Calabresa com queijo, molho e massa especial', 15.45, 'pizza-calabresa-com-queijo.jpg', 50, now(), (SELECT CategoriaId FROM Categorias WHERE Nome='Lanches'))");
            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) VALUES('Mousse de Chocolate', 'Mousse de Chocolate com raspa de limão', 6.75, 'Mousse-chocolate-com-raspa-limao.jpg', 50, now(), (SELECT CategoriaId FROM Categorias WHERE Nome='Sobremesas'))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Produtos");
        }
    }
}
