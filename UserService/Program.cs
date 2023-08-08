using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using UserService.DAL;
using UserService.Dto;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService.Services.UserService>();
builder.Services.AddDbContext<UserContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration["DB_CONNECTION_STRING"]);
});
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.Authority = "https://localhost:7256";
// Our API app will call to the IdentityServer to get the authority

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false, // Validate 
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "SampleAPI");
    });
});


var app = builder.Build();

app.UseAuthentication();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpMetrics();
app.MapMetrics();

app.UseExceptionHandler(exceptionHandlerApp
    => exceptionHandlerApp.Run(async context
        =>
    {
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();
        await Results.Problem(title: exceptionHandlerPathFeature.Error.Message)
            .ExecuteAsync(context);
    }));

app.MapGet("api/User",
        async (int userId, HttpContext context, [FromServices] UserService.Services.UserService userService) => { return await userService.GetUser(userId); })
    .WithOpenApi().ProducesProblem(500);

app.MapPost("api/User",
    async (CreateUserDto user, [FromServices] UserService.Services.UserService userService) =>
    {
        return await userService.CreateUser(user);
    }).WithOpenApi().ProducesProblem(500);

app.MapPut("api/User",
    async (UpdateUserDto user, [FromServices] UserService.Services.UserService userService) =>
    {
        return await userService.UpdateUser(user);
    }).WithOpenApi().ProducesProblem(500);


app.MapDelete("api/User", async (int userId, [FromServices] UserService.Services.UserService userService) =>
{
    await userService.DeleteUser(userId);
    return Results.StatusCode(204);
}).WithOpenApi().ProducesProblem(500);

app.Run();