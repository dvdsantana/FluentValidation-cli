using FluentValidation;
using FluentValidationCLI.Backend.Models;
using FluentValidationCLI.Backend.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Validators
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost(
        "/validate-user",
        async (IValidator<User> validator, User user) =>
        {
            var result = await validator.ValidateAsync(user);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }
            return Results.Ok("User is valid!");
        }
    )
    .WithName("ValidateUser")
    .WithOpenApi();

app.Run();
