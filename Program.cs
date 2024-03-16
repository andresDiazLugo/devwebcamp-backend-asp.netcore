using System.Text;
using dev_backend_net.Middleware;
using dev_backend_net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//configuramos cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>

    builder.WithOrigins("http://localhost:4200") // Agrega el origen de tu aplicación Angular
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
});
// configuramos en minuscula los input de la url en la que haremos la peticion
builder.Services.AddRouting(routing => routing.LowercaseUrls = true);
//haremos la conexion de la base de datos y construiremos la tablas
builder.Services.AddDbContext<UserRepository>(options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 35)));
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/home"), builder =>
   {
       builder.UseMiddleware<TokenVerificationMiddleware>();
   });
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/user/verifytoken"), builder =>
   {
       builder.UseMiddleware<TokenVerificationMiddleware>();
   });
app.UseAuthorization();

app.MapControllers();

app.Run();
