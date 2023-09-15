using Fuksi.Loyalty.Module.Auth;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddHttpClient();

services.AddCors(options =>
    options.AddDefaultPolicy(policyBuilder => policyBuilder
        .WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    )
);

services.AddAuthModule(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//await app.UseMigrations();

app.Run();