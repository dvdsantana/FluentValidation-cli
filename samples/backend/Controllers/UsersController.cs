using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Models;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IValidator<User> _validator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IValidator<User> validator, ILogger<UsersController> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new user with validation
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        var validationResult = await _validator.ValidateAsync(user);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        // In a real app, save to database here
        user.Id = Random.Shared.Next(1, 1000);

        _logger.LogInformation("Created user: {Email}", user.Email);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    /// <summary>
    /// Gets a user by ID (stub for demo purposes)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(int id)
    {
        // Stub response for demo
        return Ok(
            new User
            {
                Id = id,
                Email = "demo@example.com",
                Age = 25,
                Name = "Demo User",
            }
        );
    }

    /// <summary>
    /// Validates a user without saving (for testing validation rules)
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateUser([FromBody] User user)
    {
        var validationResult = await _validator.ValidateAsync(user);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToDictionary();
            return ValidationProblem(new ValidationProblemDetails(errors));
        }

        return Ok(new { message = "User is valid", user });
    }
}
