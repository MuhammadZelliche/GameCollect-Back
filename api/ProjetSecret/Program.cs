// --- Fichier : Program.cs (Complet avec JWT et CORS) ---

// Usings pour EF Core, JWT, Swagger, etc.
using Microsoft.EntityFrameworkCore;
using ProjetSecret.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATION DES SERVICES ---

// A. Récupérer les configurations (BDD et JWT Secret)
var connectionString = builder.Configuration.GetConnectionString("Default");
var jwtSecret = builder.Configuration["JWT_SECRET"];

if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT_SECRET n'est pas défini ! Vérifiez .env et docker-compose.yml");
}
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("ConnectionStrings:Default n'est pas défini ! Vérifiez .env et docker-compose.yml");
}

var key = Encoding.ASCII.GetBytes(jwtSecret);

// B. Ajouter le service CORS (pour autoriser les requêtes cross-domain)
builder.Services.AddCors();

// C. Ajouter le DbContext (Base de données)
builder.Services.AddDbContext<GameCollectDbContext>(options =>
    options.UseNpgsql(connectionString));

// D. Ajouter les Contrôleurs
builder.Services.AddControllers();
builder.Services.AddScoped<Seeder>();

// E. Ajouter l'Authentification JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // En dev, on n'utilise pas HTTPS forcément
    options.RequireHttpsMetadata = builder.Environment.IsProduction(); 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Pourrait être true en prod
        ValidateAudience = false // Pourrait être true en prod
    };
});

// F. Ajouter l'Autorisation
builder.Services.AddAuthorization();

// G. Ajouter Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ProjetSecret API", Version = "v1" });

    // Configurer Swagger pour utiliser le "Bearer token"
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authentification JWT (Bearer). Entrez 'Bearer' [espace] puis votre token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// --- 2. CONSTRUCTION DE L'APP ---
var app = builder.Build();

// --- 3. CONFIGURATION DU PIPELINE HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjetSecret API V1");
        c.RoutePrefix = string.Empty; // Swagger à la racine
    });
}

app.UseHttpsRedirection();

// *** POLITIQUE CORS (À PLACER ICI) ***
// (Permet à n'importe quel domaine d'accéder à l'API)
app.UseCors(options => options
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// L'ordre est crucial :
app.UseAuthentication(); // 1. Qui es-tu ?
app.UseAuthorization();  // 2. As-tu le droit ?

app.MapControllers();

// --- LOGIQUE DU SCRIPT DE SEEDING ---
// Vérifie si l'argument "seed" a été passé au lancement
if (args.Length == 1 && args[0].Equals("seed", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("Lancement du script de seeding...");

    // On crée un "scope" pour récupérer nos services
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var seeder = services.GetRequiredService<Seeder>();
            // On utilise .Wait() car on ne peut pas utiliser 'await' ici
            seeder.SeedAsync().Wait(); 
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Une erreur est survenue pendant le seeding.");
        }
    }

    // On arrête l'application après le seeding
    // On ne veut pas lancer le serveur web
    return; 
}
// --- FIN DU SCRIPT ---

app.Run();