using Allog2405.Api.Config;
using Allog2405.Api;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Allog2405.Api.Extensions;
using Allog2405.Api.DbContexts;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => {
    options.ListenLocalhost(5000);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<CustomerData>();
builder.Services.AddDbContext<CustomerContext>(options =>
{
    options.UseNpgsql("Host=localhost;Database=Univali;Username=postgres;Password=123456");
}
);

// Add services to the container.

builder.Services.AddControllers(options =>
    {
        options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
    }
).ConfigureApiBehaviorOptions(setupAction =>
       {
           setupAction.InvalidModelStateResponseFactory = context =>
           {
               // Cria a fábrica de um objeto de detalhes de problema de validação
               var problemDetailsFactory = context.HttpContext.RequestServices
                   .GetRequiredService<ProblemDetailsFactory>();


               // Cria um objeto de detalhes de problema de validação
               var validationProblemDetails = problemDetailsFactory
                   .CreateValidationProblemDetails(
                       context.HttpContext,
                       context.ModelState);


               // Adiciona informações adicionais não adicionadas por padrão
               validationProblemDetails.Detail =
                   "See the errors field for details.";
               validationProblemDetails.Instance =
                   context.HttpContext.Request.Path;


               // Relata respostas do estado de modelo inválido como problemas de validação
               validationProblemDetails.Type =
                   "https://courseunivali.com/modelvalidationproblem";
               validationProblemDetails.Status =
                   StatusCodes.Status422UnprocessableEntity;
               validationProblemDetails.Title =
                   "One or more validation errors occurred.";


               return new UnprocessableEntityObjectResult(
                   validationProblemDetails)
               {
                   ContentTypes = { "application/problem+json" }
               };
           };
       });




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.ResetDatabaseAsync();

app.Run();
