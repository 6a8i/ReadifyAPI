using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Readify.API.Common.Auth;
using Readify.CrossCutting.DependencyInjection;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configura��o da cultura
var supportedCultures = new[] { new CultureInfo("pt-BR") };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

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

builder.Services.AddSwaggerGen(options => 
{
    options.AddSecurityDefinition("Apitoken", new Microsoft.OpenApi.Models.OpenApiSecurityScheme 
    { 
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "This is an custom api authentication, just inform your token guid."

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Apitoken"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddIoC(builder.Configuration);

var app = builder.Build();

// Adiciona o middleware de localiza��o
app.UseRequestLocalization();

app.UseMiddleware<AuthMiddleware>();

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

