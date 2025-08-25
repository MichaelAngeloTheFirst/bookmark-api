using BookmarkAI_API.Consumers;
using BookmarkAI_API.Data;
using BookmarkAI_API.Services;
using BookmarkAI_API.Modules;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookmarkAI API", Version = "v1" });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme    {
        
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration["AUTHORIZATION_URL"]!),
                TokenUrl = new Uri(builder.Configuration["TOKEN_URL"]!),
                Scopes = new Dictionary<string, string>
                {
                    { "openid",  "Basic identity" },
                    { "profile", "User profile"   },
                    { "email",   "Email address"  }
                }
            }
        }
    });

    // Apply globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "openid", "profile", "email" }
        }
        });
});

builder.Services.AddControllers();
builder.Services.AddAuthentication("Clerk")
    .AddScheme<AuthenticationSchemeOptions, ClerkAuthHandler>("Clerk", null);
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});



// MassTransit
builder.Services.AddSingleton<JobScrapperService>();
builder.Services.AddSingleton<Scrapper>();
builder.Services.AddSingleton<HtmlConverter>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookmarkAI_API.Consumers.ScrapperConsumer>();
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ReceiveEndpoint("job-queue", e =>
        {
            e.ConfigureConsumer<ScrapperConsumer>(context);
        });
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.k
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookmarkAI API V1");

        
        c.OAuthClientId(builder.Configuration["OAUTH_CLIENT_ID"]!);      
        c.OAuthUsePkce();      
        c.OAuth2RedirectUrl("http://localhost:5240/swagger/oauth2-redirect.html");
        c.OAuthScopes("openid", "profile", "email");
        c.EnablePersistAuthorization();           
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
