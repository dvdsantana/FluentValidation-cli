using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Models;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IValidator<Product> _validator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IValidator<Product> validator, ILogger<ProductsController> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new product with validation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        var validationResult = await _validator.ValidateAsync(product);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        // In a real app, save to database here
        product.Id = Random.Shared.Next(1, 1000);

        _logger.LogInformation("Created product: {Name} (SKU: {Sku})", product.Name, product.Sku);

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Gets a product by ID (stub for demo purposes)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetProduct(int id)
    {
        // Stub response for demo
        return Ok(
            new Product
            {
                Id = id,
                Name = "Sample Product",
                Price = 99.99m,
                Sku = "ABC-1234",
                Description = "This is a sample product",
            }
        );
    }

    /// <summary>
    /// Validates a product without saving
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateProduct([FromBody] Product product)
    {
        var validationResult = await _validator.ValidateAsync(product);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        return Ok(new { message = "Product is valid", product });
    }
}
