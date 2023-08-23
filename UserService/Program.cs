using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using UserService.DAL;
using UserService.DAL.Model;
using UserService.Dto;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService.Services.UserService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All);
builder.Services.AddDbContext<UserContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration["DB_CONNECTION_STRING"]);
});
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    var key = Encoding.ASCII.GetBytes("suck ass asgsfsefef afafsfrsfasf sfsfsefsfdhryjtyit6u45t23423e23d2tgr23r2r22r323r23t23y23r4wgdfgdb");
    options.ClaimsIssuer = "user-service";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = "user-service",
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(key)
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

app.UseHttpLogging();
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
        async (int userId, HttpContext ctx, [FromServices] UserService.Services.UserService userService) =>
        {
            if (IsUserHasAccess(ctx, userId)) return Results.Forbid();
            return Results.Ok(await userService.GetUser(userId));
        })
    .WithOpenApi().ProducesProblem(500);

app.MapPost("api/User",
    async (CreateUserDto user, [FromServices] UserService.Services.UserService userService) =>
    {
        return await userService.CreateUser(user);
    }).WithOpenApi().ProducesProblem(500);

app.MapPut("api/User",
    async (UpdateUserDto user, HttpContext ctx, [FromServices] UserService.Services.UserService userService) =>
    {
        if (IsUserHasAccess(ctx, user.Id)) return Results.Forbid();
        return Results.Ok(await userService.UpdateUser(user));
    }).WithOpenApi().ProducesProblem(500);

app.MapPost("api/auth/token", async ([FromBody] AuthenticateRequest authenticateRequest, [FromServices] AuthService authService) =>
{
    return await authService.GenerateAuthTokenAsync(authenticateRequest);
});


app.MapDelete("api/User", async (int userId, HttpContext ctx, [FromServices] UserService.Services.UserService userService) =>
{
    if (IsUserHasAccess(ctx, userId)) return Results.Forbid();
    await userService.DeleteUser(userId);
    return Results.StatusCode(204);
}).WithOpenApi().ProducesProblem(500);


app.Run();

static bool IsUserHasAccess(HttpContext ctx, int userId)
{
    if (ctx.User.Claims.FirstOrDefault(x => x.Type == "id")?.Value != userId.ToString()) return true;
    return false;
}