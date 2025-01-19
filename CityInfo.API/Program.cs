using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using System.Reflection;
using System.Text;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

var enviornment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (enviornment == Environments.Development)
{
    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration

    .MinimumLevel.Debug()
    .WriteTo.Console());
}
else
{
   var secretClient = new SecretClient(
       new Uri("https://demo-cityinfoapi.azurewebsites.net/"),
       new DefaultAzureCredential());

    builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());

    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration

    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.ApplicationInsights(
        new TelemetryConfiguration()
        {
            InstrumentationKey = "YOUR_INSTRUMENTATION_KEY"
        },
        TelemetryConverter.Traces
        ));
}


// Add services to the container
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters(); 

builder.Services.AddProblemDetails();
//builder.Services.AddProblemDetails(options =>
//{
//    options.CustomizeProblemDetails = ctx =>
//        {
//            ctx.ProblemDetails.Extensions.Add("AdditionalInfo", "Additional info example");
//            ctx.ProblemDetails.Extensions.Add("Server", Environment.MachineName);
//        };
//}); 
//

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddSingleton<CitiesDataStore>();

builder.Services.AddDbContext<CityInfoContext>(dbContextOptions => dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]));   

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
         {
             options.TokenValidationParameters = new()
             {
                 ValidateIssuer = true,
                 ValidateAudience = true,   
                 ValidateIssuerSigningKey = true,
                 ValidIssuer = builder.Configuration["Authentication:Issuer"],
                 ValidAudience = builder.Configuration["Authentication:Audience"],
                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
             };
         }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromAntwerp", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Antwerp");
    });
});

builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.ReportApiVersions = true;
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc()
.AddApiExplorer(setupAction =>
{
    setupAction.SubstituteApiVersionInUrl = true;
});

var apiVersionDescriptionProvider = builder.Services.BuildServiceProvider()
    .GetRequiredService<IApiVersionDescriptionProvider>();

builder.Services.AddSwaggerGen(setupAction =>
{
    foreach(var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        setupAction.SwaggerDoc($"{description.GroupName}",
            new()
            { 
                Title = "City Info API",
                Version = description.ApiVersion.ToString(),
                Description = "Through this API you can access cities and their points of interest"
            });
    }

    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

    setupAction.IncludeXmlComments(xmlCommentsFullPath);

    setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            Description = "Input a valid token to access this API"
        });

    setupAction.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CityInfoApiBearerAuth"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor 
    | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

app.UseForwardedHeaders();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(setupAction =>
    {
        var descriptions = app.DescribeApiVersions();   
        foreach(var desc in descriptions)
        {
            setupAction.SwaggerEndpoint(
                $"/swagger/{desc.GroupName}/swagger.json",
                desc.GroupName.ToUpperInvariant());
        }
    });
//}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseAuthorization();

#pragma warning disable ASP0014 // Suggest using top level route registrations
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});
#pragma warning restore ASP0014 // Suggest using top level route registrations

app.MapControllers();
app.Run();
