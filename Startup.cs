using System.Text;
using bmiWebAPI_3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


namespace bmiWebAPI_3;
//change authorization services to the one from jwt authentication tutorial FBdemo
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        


        services.AddControllers();
        services.AddSingleton<ICosmosDbService>(
            InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        services.AddScoped<ITokenServices, TokenServices>();
        services.AddScoped<IPasswordService, PasswordService>();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AuthorizationWebAPI", Version = "v2"
            });
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });
            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });

        services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(config =>
        {
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = "https://localhost:7060",
                ValidAudience = "https://localhost:7060",
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("64A63153-11C1-4919-9133-EFAF99A9B456")),
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        

        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        // Shows UseCors with CorsPolicyBuilder.
        app.UseCors(builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthorizationWebAPI v2"));

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    /// <summary>
    ///     Creates a Cosmos DB database and a container with the specified partition key.
    /// </summary>
    /// <returns></returns>
    private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(
        IConfigurationSection configurationSection)
    {
        var databaseName = configurationSection.GetSection("DatabaseName").Value;
        var containerName = configurationSection.GetSection("ContainerName").Value;
        var account = configurationSection.GetSection("Account").Value;
        var key = configurationSection.GetSection("Key").Value;
        var client = new CosmosClient(account, key);
        var cosmosDbService = new CosmosDbService(client, databaseName, containerName);
        var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
        await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

        return cosmosDbService;
    }
}