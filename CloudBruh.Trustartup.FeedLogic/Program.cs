using System.Security.Cryptography;
using System.Text.Json.Serialization;
using CloudBruh.Trustartup.FeedLogic.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<FeedContentService>();
builder.Services.AddHttpClient<UserService>();
builder.Services.AddHttpClient<MediaService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        
        options.SaveToken = true;
        
        var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(
            Convert.FromBase64String(builder.Configuration.GetValue<string>("Authentication:JwtRsaPublicKey")), out _);

        var key = new RsaSecurityKey(rsa);
        
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = key,
            RequireSignedTokens = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuer = false
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();