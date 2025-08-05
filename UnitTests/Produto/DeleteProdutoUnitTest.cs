using APICatalogo.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UnitTests.Produto;

public class DeleteProdutoUnitTest(ProdutoUnitTestController controller) : IClassFixture<ProdutoUnitTestController>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.logger, controller.mapper);

    [Fact]
    public async Task DeleteProduto_ReturnsOkResult_WhenIdIsValid()
    {
        // Arrange
        int validId = 1; // Assuming this ID exists in the test database

        // Act
        var result = await _controller.Delete(validId);

        // Assert with FluentAssertions
        result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteProduto_ReturnsNotFound_WhenIdIsInvalid()
    {
        // Arrange
        int invalidId = 999; // Assuming this ID does not exist

        // Act
        var result = await _controller.Delete(invalidId);

        // Assert with FluentAssertions
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task DeleteProduto_ReturnsNotFound_WhenIdIsZero()
    {
        // Arrange
        int zeroId = default; // Invalid ID

        // Act
        var result = await _controller.Delete(zeroId);

        // Assert with FluentAssertions
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }
}