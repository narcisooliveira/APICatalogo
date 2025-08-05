using APICatalogo.Controllers;
using APICatalogo.Pagination;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace UnitTests.Produto;

public class GetProdutoUnitTests(ProdutoUnitTestController controller) : IClassFixture<ProdutoUnitTestController>
{
    private readonly ProdutosController _controller = new(controller.repository, controller.logger, controller.mapper);

    [Fact]
    public async Task GetProdutosById_ReturnsOkResult_WhenIdIsValid()
    {
        // Arrange
        int validId = 1; // Assuming this ID exists in the test database

        // Act
        var result = await _controller.GetById(validId);

        // Assert with FluentAssertions
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProdutosById_ReturnsNotFound_WhenIdIsInvalid()
    {
        // Arrange
        int invalidId = 999; // Assuming this ID does not exist

        // Act
        var result = await _controller.GetById(invalidId);
        
        // Assert with FluentAssertions
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProdutosById_ReturnsNotFound_WhenIdIsZero()
    {
        // Arrange
        int zeroId = default; // Invalid ID

        // Act
        var result = await _controller.GetById(zeroId);

        // Assert with FluentAssertions
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetListOfProdutos_ReturnOkResult_WhenCalled()
    {
        // Arrange
        var produtosParameters = new ProdutosParameters();

        // Act
        var result = await _controller.GetAll(produtosParameters);

        // Assert with FluentAssertions
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetListOfProdutos_ReturnNotFound_WhenNoProdutosExist()
    {
        // Arrange
        var produtosParameters = new ProdutosParameters
        {
            PageNumber = -1, // Invalid page number
            PageSize = 0 // Invalid page size
        };

        // Act
        var result = await _controller.GetAll(produtosParameters);

        // Assert with FluentAssertions
        result.Result.Should().BeOfType<NotFoundObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetListOfProdutos_ReturnsBadRequest_WhenParametersIsNull()
    {
        // Arrange
        ProdutosParameters? produtosParameters = null;

        // Act
        var result = await _controller.GetAll(produtosParameters);
        
        // Assert with FluentAssertions
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }
}
