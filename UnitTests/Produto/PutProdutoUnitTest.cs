using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UnitTests.Produto;

public class PutProdutoUnitTest(ProdutoUnitTestController controller) : IClassFixture<ProdutoUnitTestController>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.logger, controller.mapper);

    [Fact]
    public async Task PutProduto_ReturnsOkResult_WhenProdutoIsValid()
    {
        // Arrange
        var produto = new ProdutoDto
        {
            ProdutoId = 1, // Assuming this ID exists in the test database
            Nome = "Produto Atualizado",
            Descricao = "Descrição do Produto Atualizado",
            Preco = 150.00m,
            ImagemUrl = "http://example.com/imagem_atualizada.jpg",
            CategoriaId = 1 // Assuming this category exists in the test database
        };

        // Act
        var result = await _controller.Edit(produto.ProdutoId, produto);

        // Assert with FluentAssertions
        result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }
    [Fact]
    public async Task PutProduto_ReturnsNotFound_WhenIdIsInvalid()
    {
        // Arrange
        var produto = new ProdutoDto
        {
            ProdutoId = 999, // Assuming this ID does not exist
            Nome = "Produto Inexistente",
            Descricao = "Descrição do Produto Inexistente",
            Preco = 200.00m,
            ImagemUrl = "http://example.com/imagem_inexistente.jpg",
            CategoriaId = 1 // Assuming this category exists in the test database
        };
        
        // Act
        var result = await _controller.Edit(produto.ProdutoId, produto);
        
        // Assert with FluentAssertions
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task PutProduto_ReturnsBadRequest_WhenProdutoIsNull()
    {
        // Arrange
        ProdutoDto? produto = null;

        // Act
        var result = await _controller.Edit(produto.ProdutoId, produto);
        
        // Assert with FluentAssertions
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }
}
