using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UnitTests.Produto;

public class PostProdutoUnitTest(ProdutoUnitTestController controller) : IClassFixture<ProdutoUnitTestController>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.logger, controller.mapper);

    [Fact]
    public async Task PostProduto_ReturnsCreatedAtAction_WhenProdutoIsValid()
    {
        // Arrange
        var produto = new ProdutoDto
        {
            Nome = "Produto Teste",
            Descricao = "Descrição do Produto Teste",
            Preco = 100.00m,
            ImagemUrl = "http://example.com/imagem.jpg",
            CategoriaId = 1 // Assuming this category exists in the test database
        };

        // Act
        var result = await _controller.Create(produto);

        // Assert with FluentAssertions
        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostProduto_ReturnsBadRequest_WhenProdutoIsNull()
    {
        // Arrange
        ProdutoDto? produto = null;
        // Act
        var result = await _controller.Create(produto);
        // Assert with FluentAssertions
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }
}
