using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Readify.CrossCutting.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Adiciona o servi�o de versionamento
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0); // Define a vers�o padr�o (v1.0)
    options.AssumeDefaultVersionWhenUnspecified = true; // Usa a vers�o padr�o se n�o for especificada
    options.ReportApiVersions = true; // Informa as vers�es dispon�veis na resposta
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // L� a vers�o da URL
});

// Adiciona o servi�o de explora��o de vers�es para o Swagger
builder.Services.AddApiVersioning().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Formato das vers�es no Swagger (ex.: v1, v2)
    options.SubstituteApiVersionInUrl = true; // Substitui a vers�o na URL
});

builder.Services.AddSwaggerGen();

builder.Services.AddIoC(builder.Configuration);

var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

